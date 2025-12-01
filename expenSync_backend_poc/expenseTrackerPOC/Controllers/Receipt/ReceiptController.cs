using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Receipt.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace expenseTrackerPOC.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private IEmailService emailService;
        private IAuthService authService;
        private IReceiptService receiptService;

        public ReceiptController(IEmailService emailService, IAuthService authService, IReceiptService receiptService)
        {
            this.emailService = emailService;
            this.authService = authService;
            this.receiptService = receiptService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> ProcessReceipt(IFormFile file)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new UploadFileResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = int.Parse(Id);

            var uploadResult = await receiptService.UploadReceiptAsync(file, userId);
            
            if(!uploadResult.success)
            {
                return BadRequest(new UploadFileResponse
                {
                    Success = false,
                    Message = "Unable to process your file for now",
                    Errors = new[] { uploadResult.message }
                }); 
            }

            return Ok(uploadResult.uploadFileResponse);
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanReceipt(IFormFile file)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new UploadFileResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = int.Parse(Id);

            var uploadResult = await receiptService.ScanReceiptAsync(file, userId);

            if (!uploadResult.success)
            {
                return BadRequest(new UploadFileResponse
                {
                    Success = false,
                    Message = "Unable to process your file for now",
                    Errors = new[] { uploadResult.message }
                });
            }

            return Ok(uploadResult.uploadFileResponse);
        }

    }
}
