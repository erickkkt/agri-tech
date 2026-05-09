namespace Farm.Api.Services
{
    public interface IMediaStorageService
    {
        /// <summary>
        /// Stores the given stream and returns a publicly-resolvable URL plus
        /// the storage key (so the caller can persist either reference).
        /// </summary>
        /// <param name="kind">Logical bucket: AnimalPhoto, ListingPhoto, Contract, CameraSnapshot.</param>
        Task<(string url, string key)> UploadAsync(string kind, string fileName, string contentType, Stream content, CancellationToken ct = default);

        Task<bool> DeleteAsync(string key, CancellationToken ct = default);
    }
}
