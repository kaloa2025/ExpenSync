using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Receipt.Interfaces;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Identity.Client;

namespace expenseTrackerPOC.Services.Receipt
{
    public class ReceiptService : IReceiptService
    {
        public readonly string[] allowdedExtensions = { ".jpg", ".jpeg", ".png", "/jpg", "/jpeg", "/png" };
        private const long MaxFileSize = 5 * 1024 * 1024;

        public readonly ITransactionService transactionService;
        private readonly IBlobService blobService;

        private readonly string documentEndpoint;
        private readonly string documentKey;

        public ReceiptService(IBlobService blobService, ITransactionService transactionService, IConfiguration configuration)
        {
            this.documentEndpoint = configuration["AzureDocument:Endpoint"];
            this.documentKey = configuration["AzureDocument:ApiKey"];
            this.transactionService = transactionService;
            this.blobService = blobService;
        }
        public async Task<(bool success, string message, UploadFileResponse uploadFileResponse)> UploadReceiptAsync(IFormFile file, int userId)
        {
            //0. Validate file
            var validation = ValidateFile(file);
            if (!validation.IsValid)
            {
                return (false, validation.Error, null);
            }
            //1. Create File name
            string fileName = GenerateFileName(userId, file);
            //2. Add the file to blob
            var blobUpload = await blobService.UploadImageToBlob(fileName, file);
            if (!blobUpload.Success)
            {
                return (false, blobUpload.Message, null);
            }
            string blobUrl = blobUpload.BlobUrl;
            //3. Send blob to Document Intelligence
            var expenseData = await ExtractReceiptDataAsync(blobUrl, userId);
            if (!expenseData.Success)
            {
                return (false, "Failed to extract receipt details.", null);
            }
            //4. Return 
            return (true, "Receipt processed successfully", expenseData.Data);
        }
        public async Task<(bool success, string message, ScanReceiptResponse scanReceiptResponse)> ScanReceiptAsync(IFormFile file, int userId)
        {
            //0. Validate file
            var validation = ValidateFile(file);
            if (!validation.IsValid)
            {
                return (false, validation.Error, null);
            }
            //1. Create File name
            string fileName = GenerateFileName(userId, file);
            //2. Add the file to blob
            var blobUpload = await blobService.UploadImageToBlob(fileName, file);
            if (!blobUpload.Success)
            {
                return (false, blobUpload.Message, null);
            }
            string blobUrl = blobUpload.BlobUrl;
            //3. Send blob to Document Intelligence
            var expenseData = await ExtractReceiptJsonAsync(blobUrl, userId);
            if (!expenseData.Success)
            {
                return (false, "Failed to extract receipt details.", null);
            }
            //4. Send Json to AI to propogate fields with context
            var result = await PropogateWithAI(expenseData.jsonData);
            if (!result.Success)
            {
                return (false, "Failed to propogate fields", null);
            }

            //5. Return final result
            return (true, "Receipt processed successfully", expenseData.Data);
        }


        private (bool IsValid, string Error) ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File doesn't exist");
            }
            if (file.Length > MaxFileSize)
            {
                return (false, "File too large, Max 5MB allowded");
            }
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowdedExtensions.Contains(ext))
            {
                return (false, "Invalid file Type, Only JPG, JPEG, PNG formats are allowed.");
            }
            return (true, null);
        }

        private string GenerateFileName(int userId, IFormFile file)
        {
            string ext = Path.GetExtension(file.FileName).ToLower();
            return $"{userId}/{DateTime.UtcNow}_{Guid.NewGuid()}{ext}";
        }

        
        private async Task<(bool Success, string jsonData)> ExtractReceiptJsonAsync(string blobUrl, int userId)
        {
            var responseModel = new ScanReceiptResponse
            {
                Success = false
            };
        }

        private async Task<(bool Success, UploadFileResponse Data)> ExtractReceiptDataAsync(string blobUrl, int userId)
        {
            var responseModel = new UploadFileResponse
            {
                UserId = userId,
                Success = false
            };

            var client = new DocumentIntelligenceClient(
                new Uri(documentEndpoint),
                new AzureKeyCredential(documentKey));

            using var http = new HttpClient();
            var bytes = await http.GetByteArrayAsync(blobUrl);
            var binary = new BinaryData(bytes);

            var operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-receipt",
                binary
            );

            var result = operation.Value;

            if (result.Documents.Count == 0)
                return (false, responseModel);

            var fields = result.Documents[0].Fields;

            // ---- TOTAL ----
            decimal total = 0;
            if (fields.TryGetValue("Total", out var totalField))
            {
                var raw = totalField.Content ?? "";
                raw = raw.Replace("CAD", "")
                         .Replace("$", "")
                         .Trim();
                decimal.TryParse(raw, out total);
            }
            if (fields.TryGetValue("transactionAmount", out var amountField))
            {
                var raw = amountField.Content ?? "";
                raw.Remove(0, 1);
                raw = raw.Replace("CAD", "")
                         .Replace("$", "")
                         .Trim();
                decimal.TryParse(raw, out total);
            }
            if (fields.TryGetValue("SubTotal", out var subField))
            {
                decimal tax = 0;
                if (fields.TryGetValue("TaxDetails", out var tdses))
                {
                    decimal.TryParse(tdses.Content, out tax);
                }
                decimal.TryParse(subField.Content+tax, out total);
            }

            // ---- MERCHANT ----
            string merchantName = 
                fields.TryGetValue("reciverSenderName", out var rsField)? rsField.Content
                : fields.TryGetValue("MerchantName", out var nameField)? nameField.Content
                : fields.TryGetValue("MerchantAddress", out var addrField)? addrField.Content
                : "";

            string merchantDescription= fields.TryGetValue("ReceiptType", out var items)
                ? items.ValueString
                : "";

            // ---- DATE + TIME ----
            DateTime? date = fields.TryGetValue("TransactionDate", out var dateField)
                ? dateField.ValueDate?.UtcDateTime
                : null;

            if (fields.TryGetValue("TransactionTime", out var timeField)
                && date.HasValue)
            {
                if (DateTime.TryParse(timeField.Content, out var timeValue))
                {
                    date = date.Value.Date.Add(timeValue.TimeOfDay);
                }
            }

            responseModel.TransactionAmount = total;
            responseModel.ReciverSenderName = merchantName;
            responseModel.TransactionDescription = merchantDescription;
            responseModel.TransactionDate = date ?? DateTime.Now;

            responseModel.Success = true;
            responseModel.Message = "Extraction successful";

            return (true, responseModel);
        }
    }
    }
