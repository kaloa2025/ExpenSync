using expenseTrackerPOC.Data.ResponseModels;

namespace expenseTrackerPOC.Services.Receipt.Interfaces
{
    public interface IReceiptService
    {
        public Task<(bool success, string message, UploadFileResponse uploadFileResponse)> UploadReceiptAsync(IFormFile file, int id);
        public Task<(bool success, string message, ScanReceiptResponse scanReceiptResponse)> ScanReceiptAsync(IFormFile file, int id);
    }
}

