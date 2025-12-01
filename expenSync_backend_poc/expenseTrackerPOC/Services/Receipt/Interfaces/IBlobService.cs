using expenseTrackerPOC.Services.Receipt;

namespace expenseTrackerPOC.Services.Receipt.Interfaces
{
    public interface IBlobService
    {
        public Task<(bool Success, string Message, string BlobUrl)> UploadImageToBlob(string fileName, IFormFile file);
    }
}