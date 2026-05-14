using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Farm.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Farm.Api.Services
{
    /// <summary>
    /// HS256 JWT issuer for local end-user auth. Configured via "Jwt:Issuer",
    /// "Jwt:Audience", "Jwt:SigningKey" and "Jwt:AccessTokenMinutes" in appsettings.
    ///
    /// Claim shape is intentionally compatible with HttpContextExtension.GetUserProfile():
    /// - objectidentifier  → user.Id
    /// - name              → DisplayName ?? UserName
    /// - preferred_username → EmailAddress
    /// so all existing controllers reading AuthorizedUser keep working unchanged.
    /// </summary>
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public JwtTokenService(IConfiguration config) => _config = config;

        public (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
        {
            var issuer = _config.GetValue<string>("Jwt:Issuer") ?? "agri-tech-api";
            var audience = _config.GetValue<string>("Jwt:Audience") ?? "agri-tech-webuser";
            var signingKey = _config.GetValue<string>("Jwt:SigningKey")
                ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");
            var minutes = _config.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 60;

            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var claims = new List<Claim>
            {
                // Compatible with existing GetUserProfile() extension
                new("http://schemas.microsoft.com/identity/claims/objectidentifier", user.Id.ToString()),
                new("name", string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName),
                new("preferred_username", user.EmailAddress ?? string.Empty),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.EmailAddress ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expires);
        }
    }
}
