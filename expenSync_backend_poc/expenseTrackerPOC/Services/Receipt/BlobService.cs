using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using expenseTrackerPOC.Services.Receipt.Interfaces;
using System.Net.Http.Headers;

namespace expenseTrackerPOC.Services.Receipt
{
    public class BlobService : IBlobService
    {
        private const string ContainerName = "receipts";

        private readonly BlobContainerClient containerClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            containerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }
        public async Task<(bool Success, string Message, string BlobUrl)> UploadImageToBlob(string fileName, IFormFile file)
        {
            try
            {
                var blobClient = containerClient.GetBlobClient(file.FileName);
                using var stream = file.OpenReadStream();

                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType,
                    }
                });

                return (true, "Upload Successfull", blobClient.Uri.ToString());
            }
            catch (Exception ex)
            {
                return (false, $"Failed to upload Image: {ex.Message}", null);
            }
        }
    }
}
