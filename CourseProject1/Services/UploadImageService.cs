using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using CourseProject1.ServiceContracts;
using System.Configuration;

namespace CourseProject1.Services
{
    public class UploadImageService : IUploadImage
    {
        private readonly IConfiguration _configuration;
        private SecretClient _secretClient;

        public UploadImageService(SecretClient secretClient, IConfiguration configuration)
        {
            _secretClient = secretClient;
            _configuration = configuration;
        }
        public async Task<string> UploadImageToCloudStorage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }
            try
            {

                KeyVaultSecret azureConnectionStringSecret = await _secretClient.GetSecretAsync("AzureConnectionString");
                string connectionString = azureConnectionStringSecret.Value;

                string containerName = _configuration.GetValue<string>("StorageConfig:ContainerName");

                // Create a BlobServiceClient object
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get a reference to the container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Create a unique name for the blob
                string blobName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

                // Get a reference to the blob
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Upload the image file to Azure Blob Storage
                using (Stream stream = imageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                // Get the URL of the uploaded image
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the upload process
                // For simplicity, you can log the error or return null
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
        }
    }
}
