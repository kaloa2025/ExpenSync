using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.AI.OpenAI;
using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Receipt.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

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

        private readonly string openAIEndpoint;
        private readonly string openAIKey;

        private ExpenseTrackerDbContext dbContext { get; set; }

        public ReceiptService(IBlobService blobService, ITransactionService transactionService, IConfiguration configuration, ExpenseTrackerDbContext dbContext)
        {
            this.documentEndpoint = configuration["AzureDocument:Endpoint"];
            this.documentKey = configuration["AzureDocument:ApiKey"];

            this.openAIKey = configuration["AzureOpenAI:ApiKey"];
            this.openAIEndpoint = configuration["AzureOpenAI:Endpoint"];

            this.transactionService = transactionService;
            this.blobService = blobService;
            this.dbContext = dbContext;
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
            
            //3. Send file to AI to propogate fields with context
            var result = await ExtractUsingAI(blobUrl, userId);
            if (!result.Success)
            {
                return (false, "Failed to Extract fields", null);
            }

            //5. Return final result
            return (true, "Receipt processed successfully", result.Data);
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

        private async Task<(bool Success, ScanReceiptResponse Data)> ExtractUsingAI(string blobUrl, int userId)
        {
            try
            {
                var client = new AzureOpenAIClient(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));

                Dictionary<int, string> categories = await dbContext.Categories.Where(c => c.IsDefault == 1 || c.UserId == userId).ToDictionaryAsync(c => c.CategoryId, c => c.CategoryName);
                Dictionary<int, string> expenseTypes = await dbContext.ExpenseTypes.ToDictionaryAsync(c=>c.ExpenseTypeId, c => c.ExpenseTypeName);
                Dictionary<int, string> paymentModes = await dbContext.ModeOfPayments.ToDictionaryAsync(c => c.ModeOfPaymentId, c => c.ModeOfPaymentName);

                string categoriesText = string.Join("\n", categories.Select(x => $"{x.Key}: {x.Value}"));
                string expenseTypesText = string.Join("\n", expenseTypes.Select(x => $"{x.Key}: {x.Value}"));
                string paymentModesText = string.Join("\n", paymentModes.Select(x => $"{x.Key}: {x.Value}"));

                string gptSystemPrompt = @$"
                You are an expert financial assistant with OCR vision capabilities.
                Extract the following fields accurately:
                - transactionAmount (numeric only)
                - transactionDate (convert to yyyy-MM-dd HH:mm:ss)
                - transactionDescription
                - reciverSenderName
                - categoryId (choose the BEST match from list below)
                - modeOfPaymentId (select from list below)
                - expenseTypeId (select from list below)

                CATEGORIES:
                {categoriesText}

                EXPENSE TYPES:
                {expenseTypesText}

                PAYMENT MODES:
                {paymentModesText}

                Respond ONLY with valid JSON in the following structure:
                {{
                    ""transactionAmount"": number,
                    ""transactionDate"": ""yyyy-MM-dd HH:mm:ss"",
                    ""transactionDescription"": ""string"",
                    ""reciverSenderName"": ""string"",
                    ""categoryId"": number,
                    ""modeOfPaymentId"": number,
                    ""expenseTypeId"": number
                }}
                ";

                var chatClient = client.GetChatClient("gpt-4o-mini");

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(gptSystemPrompt),
                    new UserChatMessage(new List<ChatMessageContentPart>
                    {
                        ChatMessageContentPart.CreateTextPart("Extract transaction details from this receipt image"),
                        ChatMessageContentPart.CreateImagePart(new Uri(blobUrl))
                    })
                };

                var result = await chatClient.CompleteChatAsync(messages);

                string content = result.Value.Content[0].Text;

                var extractedJson = ExtractOnlyJson(content);
                var finalObj = JsonConvert.DeserializeObject<ScanReceiptResponse>(extractedJson);

                if (finalObj.CategoryId == 0 || !categories.ContainsKey(finalObj.CategoryId))
                    finalObj.CategoryId = categories.First().Key;

                if (finalObj.ExpenseTypeId == 0 || !expenseTypes.ContainsKey(finalObj.ExpenseTypeId))
                    finalObj.ExpenseTypeId = expenseTypes.First().Key;

                if (finalObj.ModeOfPaymentId == 0 || !paymentModes.ContainsKey(finalObj.ModeOfPaymentId))
                    finalObj.ModeOfPaymentId = paymentModes.First().Key;

                finalObj.Success = true;
                finalObj.Message = "AI Extraction Completed";

                return (true, finalObj);
            }
            catch(Exception ex) 
            {
                return (false, new ScanReceiptResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        private string ExtractOnlyJson(string content)
        {
            var match = Regex.Match(content, "{[\\s\\S]*}");
            return match.Success ? match.Value : content;
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
