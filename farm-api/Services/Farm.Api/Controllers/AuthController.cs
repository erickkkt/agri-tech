using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Farm.Api.Services;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// End-user authentication for the web-user app (marketplace, investment).
    /// Admins still authenticate via Azure AD B2C — those JWTs are validated by
    /// the "AzureAD" / "AzureAD_B2C" schemes configured in Program.cs and are
    /// independent of this controller.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FarmDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            FarmDbContext db,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthController> logger)
        {
            _db = db;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>Create a new end-user account and return a JWT so the client logs in immediately.</summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            if (req == null) return BadRequest(new { error = "Yêu cầu trống." });

            var email = NormalizeEmail(req.Email);
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { error = "Email không được để trống." });
            if (!IsValidEmail(email))
            {
                // Log the rejected value so we can see invisible chars / typos
                // when end users report "email không hợp lệ" on what looks like a valid address.
                _logger.LogWarning("Register rejected — invalid email format. Raw={Raw} Normalized={Normalized}",
                    req.Email, email);
                return BadRequest(new { error = "Email không hợp lệ." });
            }
            if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                return BadRequest(new { error = "Mật khẩu phải có ít nhất 6 ký tự." });

            // Duplicate check (the DB also has IX_User_EmailAddress_Unique as a safety net).
            var exists = await _db.Users.AnyAsync(u => u.EmailAddress == email);
            if (exists)
                return BadRequest(new { error = "Email này đã được đăng ký." });

            var displayName = string.IsNullOrWhiteSpace(req.DisplayName)
                ? email.Split('@')[0]
                : req.DisplayName.Trim();

            var user = new User
            {
                UserName = email,
                EmailAddress = email,
                DisplayName = displayName,
                PhoneNumber = req.PhoneNumber?.Trim(),
                AuthProvider = AuthProvider.Local,
                IsActive = true,
                RoleId = null,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = Guid.Empty,
                ChangedByUserName = "self-register"
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);

            _db.Users.Add(user);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505")
            {
                // Race condition or stale duplicate slipped past the AnyAsync check.
                return BadRequest(new { error = "Email này đã được đăng ký." });
            }
            catch (DbUpdateException ex)
            {
                // Other DB failures (missing column, NOT NULL violation, FK error…) — surface them
                // explicitly instead of letting the request bubble to GlobalExceptionHandler as a
                // generic 500. Most common cause here: the AddEndUserAuthFields migration hasn't
                // been applied yet so the new columns don't exist in the DB.
                _logger.LogError(ex, "Register failed at SaveChanges for {Email}. SqlState={SqlState} Message={Inner}",
                    email,
                    (ex.InnerException as Npgsql.PostgresException)?.SqlState,
                    ex.InnerException?.Message);
                return StatusCode(500, new
                {
                    error = "Lưu tài khoản thất bại. Vui lòng thử lại hoặc liên hệ admin.",
                    detail = ex.InnerException?.Message
                });
            }

            try
            {
                _logger.LogInformation("Registered new end-user {Email} ({Id})", user.EmailAddress, user.Id);
                return Ok(BuildResponse(user));
            }
            catch (Exception ex)
            {
                // BuildResponse → JwtTokenService.GenerateAccessToken throws if Jwt:SigningKey
                // is missing in config. The user IS already saved at this point, so make the
                // failure visible rather than hiding it as a generic 500.
                _logger.LogError(ex, "Register: user persisted but JWT issuance failed for {Email}", email);
                return StatusCode(500, new
                {
                    error = "Đăng ký thành công nhưng không tạo được phiên đăng nhập. Vui lòng đăng nhập lại.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>Verify password and return a JWT.</summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            if (req == null) return BadRequest(new { error = "Yêu cầu trống." });

            var email = NormalizeEmail(req.Email);
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Email hoặc mật khẩu không được để trống." });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailAddress == email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                // Same response for "no such user" and "user uses social login" so we don't
                // leak account existence.
                return Unauthorized(new { error = "Email hoặc mật khẩu không đúng." });
            }
            if (!user.IsActive)
                return Unauthorized(new { error = "Tài khoản đã bị khoá." });

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Unauthorized(new { error = "Email hoặc mật khẩu không đúng." });

            // Opportunistic rehash if the hash format is outdated (e.g. older iteration count).
            if (verify == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, req.Password);
                await _db.SaveChangesAsync();
            }

            return Ok(BuildResponse(user));
        }

        // ---------- Email helpers ----------

        // Practical email regex — accepts hyphens, dots, plus, and digits in the local part
        // (e.g. agri-tech-001@yopmail.com, user+tag@gmail.com). Deliberately not full RFC 5322.
        private static readonly Regex EmailRegex = new(
            @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Strip whitespace and zero-width / invisible characters that often sneak in from
        /// copy-paste (ZWSP U+200B, NBSP U+00A0, BOM U+FEFF…) before validating + lowercase.
        /// </summary>
        private static string NormalizeEmail(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            var cleaned = new System.Text.StringBuilder(raw.Length);
            foreach (var ch in raw)
            {
                if (char.IsWhiteSpace(ch)) continue;
                if (char.IsControl(ch)) continue;
                if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.Format) continue;
                cleaned.Append(ch);
            }
            return cleaned.ToString().ToLowerInvariant();
        }

        private static bool IsValidEmail(string email)
            => !string.IsNullOrWhiteSpace(email)
               && email.Length <= 254
               && EmailRegex.IsMatch(email);

        private AuthResponse BuildResponse(User user)
        {
            var (token, expiresAt) = _tokenService.GenerateAccessToken(user);
            return new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.EmailAddress,
                    DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName
                }
            };
        }
    }

    // ---------- DTOs ----------

    public class RegisterRequest
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class LoginRequest
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public AuthUserDto User { get; set; } = new();
    }

    public class AuthUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
