using expenseTrackerPOC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class UploadFileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[]? Errors { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? TransactionDescription { get; set; }
        public string ReciverSenderName { get; set; }
        public DateTime TransactionDate { get; set; }
        public int UserId { get; set; }
    }
}