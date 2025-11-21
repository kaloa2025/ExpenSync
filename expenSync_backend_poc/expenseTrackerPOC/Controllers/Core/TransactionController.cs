using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace expenseTrackerPOC.Controllers.Core
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private IEmailService emailService;
        private IAuthService authService;
        private ICategoryService categoryService;
        private ITransactionService transactionService;

        public TransactionController(IEmailService emailService, IAuthService authService, ICategoryService categoryService, ITransactionService transactionService)
        {
            this.transactionService = transactionService;
            this.emailService = emailService;
            this.authService = authService;
            this.categoryService = categoryService;
        }


        [HttpPost("AddNewExpense")]
        public async Task<ActionResult<AddNewExpenseResponse>> AddNewExpense([FromBody] AddNewExpenseRequest addNewExpenseRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AddNewExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Input Values"
                });
            }

            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new AddNewExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var added_newExpense = await transactionService.AddNewExepense(userId, addNewExpenseRequest);

            if (!added_newExpense.Success)
            {
                return BadRequest(added_newExpense);
            }

            return Ok(added_newExpense);
        }

        [HttpGet("GetRecentTransactions")]
        public async Task<ActionResult<GetRecentTransactionsResponse>> GetRecentTransactions()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new GetRecentTransactionsResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var recentTransactions = await transactionService.GetRecentTransactions(userId);

            if(!recentTransactions.Success)
            {
                return BadRequest(recentTransactions);
            }

            return Ok(recentTransactions);
        }

        [HttpGet("GetReport")]
        public async Task<ActionResult<GetReportResponse>> GetReport()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new GetReportResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var reportTransactions = await transactionService.GetReport(userId);
            if (!reportTransactions.Success)
            {
                return BadRequest(reportTransactions);
            }

            return Ok(reportTransactions);
        }

        [HttpGet("GetGraphData")]
        public async Task<ActionResult<GetGraphDataResponse>> GetGraphData()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new GetGraphDataResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var reportTransactions = await transactionService.GetGraphData(userId);
            if (!reportTransactions.Success)
            {
                return BadRequest(reportTransactions);
            }

            return Ok(reportTransactions);
        }

        [HttpDelete("DeleteTransaction/{transactionId}")]
        public async Task<ActionResult<DeleteExpenseResponse>> DeleteExpense(int transactionId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DeleteExpenseResponse
                {
                    Success = false,
                    Message = "Invalid requested Values"
                });
            }

            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new DeleteExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var deletedExpense = await transactionService.DeleteExepense(userId, transactionId);

            if (!deletedExpense.Success)
            {
                return BadRequest(deletedExpense);
            }

            return Ok(deletedExpense);
        }

        [HttpPut("EditExpense/{transactionId}")]
        public async Task<ActionResult<EditExpenseResponse>> EditExpense(int transactionId, [FromBody] EditExpenseRequest editExpenseRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new EditExpenseResponse
                {
                    Success = false,
                    Message = "Invalid requested Values"
                });
            }

            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new EditExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var editedExpense = await transactionService.EditExpense(userId, transactionId, editExpenseRequest);

            if (!editedExpense.Success)
            {
                return BadRequest(editedExpense);
            }

            return Ok(editedExpense);
        }
    }
}