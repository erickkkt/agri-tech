using Farm.Api.Services;
using Farm.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>Upload entry point for animal photos, listing photos, contract docs, etc.</summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/media")]
    [Produces("application/json")]
    [ApiController]
    public class MediaController : FarmBaseController
    {
        private readonly IMediaStorageService _storage;
        private const long MAX_BYTES = 25 * 1024 * 1024; // 25 MB

        private static readonly HashSet<string> ALLOWED_KINDS = new(StringComparer.OrdinalIgnoreCase)
        {
            "AnimalPhoto", "ListingPhoto", "Contract", "CameraSnapshot", "GrowthLogPhoto"
        };

        public MediaController(IMediaStorageService storage, IUserService userService) : base(userService)
        {
            _storage = storage;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MAX_BYTES)]
        public async Task<IActionResult> Upload([FromQuery] string kind, IFormFile file)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(kind) || !ALLOWED_KINDS.Contains(kind))
                return BadRequest(new { error = "Invalid kind." });

            if (file == null || file.Length == 0) return BadRequest(new { error = "Empty file." });
            if (file.Length > MAX_BYTES) return BadRequest(new { error = "File too large." });

            using var stream = file.OpenReadStream();
            var (url, key) = await _storage.UploadAsync(kind, file.FileName, file.ContentType, stream, HttpContext.RequestAborted);
            return Ok(new { url, key, size = file.Length, contentType = file.ContentType });
        }
    }
}
