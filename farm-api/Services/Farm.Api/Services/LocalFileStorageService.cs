using Microsoft.AspNetCore.Hosting;

namespace Farm.Api.Services
{
    /// <summary>
    /// Simple file storage implementation for development.
    /// Saves under wwwroot/uploads/{kind}/{yyyy}/{MM}/{guid}_{filename}.
    /// </summary>
    public class LocalFileStorageService : IMediaStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContext;
        private const string ROOT_FOLDER = "uploads";

        public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContext)
        {
            _env = env;
            _httpContext = httpContext;
        }

        public async Task<(string url, string key)> UploadAsync(string kind, string fileName, string contentType, Stream content, CancellationToken ct = default)
        {
            var safeKind = SanitizeKind(kind);
            var now = DateTime.UtcNow;
            var relativeDir = Path.Combine(ROOT_FOLDER, safeKind, now.Year.ToString(), now.Month.ToString("00"));
            var absoluteDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relativeDir);
            Directory.CreateDirectory(absoluteDir);

            var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
            var fullPath = Path.Combine(absoluteDir, safeName);
            using (var fs = new FileStream(fullPath, FileMode.CreateNew))
            {
                await content.CopyToAsync(fs, ct);
            }

            var key = Path.Combine(relativeDir, safeName).Replace("\\", "/");
            var request = _httpContext.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : string.Empty;
            var url = $"{baseUrl}/{key}";
            return (url, key);
        }

        public Task<bool> DeleteAsync(string key, CancellationToken ct = default)
        {
            var path = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), key.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path))
            {
                File.Delete(path);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private static string SanitizeKind(string kind)
        {
            if (string.IsNullOrWhiteSpace(kind)) return "general";
            var clean = new string(kind.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
            return string.IsNullOrEmpty(clean) ? "general" : clean.ToLowerInvariant();
        }
    }
}
