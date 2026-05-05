using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _containerName = configuration["BlobStorage:ContainerName"] ?? "venue-images";
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Create container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var uri = new Uri(imageUrl);
            var fileName = Path.GetFileName(uri.LocalPath);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}