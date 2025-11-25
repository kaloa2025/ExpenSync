using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace expenseTrackerPOC.Services.Core
{
    public class TransactionService : ITransactionService
    {
        private ExpenseTrackerDbContext dbContext { get; set; }
        public TransactionService(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AddNewExpenseResponse> AddNewExepense(int userId, AddNewExpenseRequest addNewExpenseRequest)
        {
            Transaction transaction = new Transaction
            {
                TransactionDescription = addNewExpenseRequest.TransactionDescription,
                ReciverSenderName = addNewExpenseRequest.ReciverSenderName,
                TransactionAmount = addNewExpenseRequest.TransactionAmount,
                UserId = userId,
                CategoryId = addNewExpenseRequest.CategoryId,
                ExpenseTypeId = addNewExpenseRequest.ExpenseTypeId,
                ModeOfPaymentId = addNewExpenseRequest.ModeOfPaymentId,
                TransactionDate = addNewExpenseRequest.TransactionDate
            };

            //1. Add Category
            var transactionAdded = (await dbContext.Transactions.AddAsync(transaction)).Entity;
            await dbContext.SaveChangesAsync();

            if (transactionAdded == null)
            {
                return new AddNewExpenseResponse
                {
                    Success = false,
                    Message = "Couldn't Add New Transaction, Try again later."
                };
            }

            return new AddNewExpenseResponse
            {
                Success = true,
                Message = "Transaction Added Scuccessfully",
                transaction = transactionAdded
            };

        }

        public async Task<GetRecentTransactionsResponse> GetRecentTransactions(int userId)
        {
            try
            {
                var transactions = await dbContext.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.TransactionDate).Take(5).ToListAsync();

                if (transactions == null || !transactions.Any())
                {
                    return new GetRecentTransactionsResponse
                    {
                        Success = false,
                        Message = "No recent transactions found.",
                        transactionsByDate = new Dictionary<DateTime, List<Transaction>>()
                    };
                }

                var groupedTransactions = transactions.GroupBy(t => t.TransactionDate.Date).ToDictionary(g => g.Key, g => g.ToList());

                return new GetRecentTransactionsResponse
                {
                    Success = true,
                    Message = "Recent transactions fetched successfully.",
                    transactionsByDate = groupedTransactions
                };
            }
            catch (Exception ex)
            {
                return new GetRecentTransactionsResponse
                {
                    Success = false,
                    Message = "An error occurred while fetching recent transactions.",
                    Errors = new List<string> { ex.Message },
                    transactionsByDate = new Dictionary<DateTime, List<Transaction>>()
                };
            }
        }


        public async Task<GetReportResponse> GetReport(int userId)
        {
            try
            {
                var transactions = await dbContext.Transactions
                    .Include(t => t.Category)
                    .Include(t => t.ExpenseType)
                    .Include(t => t.ModeOfPayment)
                    .Where(t => t.UserId == userId)
                    .Select(t => new TransactionReportDto
                    {
                        TransactionId = t.TransactionId,
                        TransactionDescription = t.TransactionDescription,
                        ReciverSenderName = t.ReciverSenderName,
                        TransactionAmount = t.TransactionAmount,
                        TransactionDate = t.TransactionDate,
                        CategoryName = t.Category.CategoryName,
                        ExpenseTypeName = t.ExpenseType.ExpenseTypeName,
                        ModeOfPaymentName = t.ModeOfPayment.ModeOfPaymentName
                    })
                    .ToListAsync();
                if (transactions == null || !transactions.Any())
                {
                    return new GetReportResponse
                    {
                        Success = false,
                        Message = "No transactions found for report generation.",
                        transactions = new Dictionary<DateTime, List<TransactionReportDto>>()
                    };
                }
                //TransactionReportDto
                var monthlyGrouped = transactions.GroupBy(t => new DateTime(t.TransactionDate.Year, t.TransactionDate.Month, 1)).ToDictionary(g => g.Key, g => g.ToList());

                return new GetReportResponse
                {
                    Success = true,
                    Message = "Report generated successfully.",
                    transactions = monthlyGrouped
                };
            }
            catch (Exception ex)
            {
                return new GetReportResponse
                {
                    Success = false,
                    Message = "An error occurred while generating the report.",
                    Errors = new List<string> { ex.Message },
                    transactions = new Dictionary<DateTime, List<TransactionReportDto>>()
                };
            }
        }

        public async Task<GetGraphDataResponse> GetGraphData(int userId)
        {
            try
            {
                var transactions = await dbContext.Transactions.Include(t => t.Category).Where(t => t.UserId == userId).ToListAsync();

                if (transactions == null || !transactions.Any())
                {
                    return new GetGraphDataResponse
                    {
                        Success = false,
                        Message = "No transactions found for expense summary.",
                        Data = new Dictionary<string, decimal>()
                    };
                }

                var summary = transactions.GroupBy(t => t.Category.CategoryName).ToDictionary(g => g.Key, g => g.Sum(t => t.TransactionAmount));

                return new GetGraphDataResponse
                {
                    Success = true,
                    Message = "Expense summary fetched successfully.",
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                return new GetGraphDataResponse
                {
                    Success = false,
                    Message = "Error occurred while fetching expense summary.",
                    Data = new Dictionary<string, decimal>(),
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<DeleteExpenseResponse> DeleteExepense(int userId, int transactionId)
        {
            //1. Find Sepecific 
            if (transactionId == 0)
            {
                return new DeleteExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Transaction Selected"
                };
            }

            var transaction = await dbContext.Transactions.FirstOrDefaultAsync(u => u.TransactionId == transactionId && u.UserId == userId);
            if (transaction == null)
            {
                return new DeleteExpenseResponse
                {
                    Success = false,
                    Message = "Unable to locate specific Transaction for user, Try again later"
                };
            }

            //2. Delete
            dbContext.Transactions.Remove(transaction);
            await dbContext.SaveChangesAsync();

            return new DeleteExpenseResponse
            {
                Success = true,
                Message = "Transaction Deleted Successfully!"
            };
        }

        public async Task<EditExpenseResponse> EditExpense(int userId, int transactionId, EditExpenseRequest editExpenseRequest)
        {
            //1. Find Expense to edit
            if (transactionId == 0)
            {
                return new EditExpenseResponse
                {
                    Success = false,
                    Message = "Invalid Transaction Selected"
                };
            }

            var transaction = dbContext.Transactions.FirstOrDefault(u=>u.TransactionId == transactionId && u.UserId == userId);
            if (transaction == null)
            {
                return new EditExpenseResponse
                {
                    Success = false,
                    Message = "Unable to locate specific Transaction for user, Try again later"
                };
            }
            //2. Update Changes

            transaction.TransactionDescription = editExpenseRequest.TransactionDescription;
            transaction.ReciverSenderName = editExpenseRequest.ReciverSenderName;
            transaction.TransactionAmount = editExpenseRequest.TransactionAmount;
            transaction.TransactionDate = editExpenseRequest.TransactionDate;
            transaction.CategoryId = editExpenseRequest.CategoryId;
            transaction.ModeOfPaymentId = editExpenseRequest.ModeOfPaymentId;
            transaction.ExpenseTypeId = editExpenseRequest.ExpenseTypeId;

            await dbContext.SaveChangesAsync();

            //3. Inform
            return new EditExpenseResponse
            {
                Success = true,
                Message = "Transaction updated Successfully!"
            };
        }

        public async Task<List<ExpenseType>> GetAllExpenseTypes()
        {
            var expenseList = await dbContext.ExpenseTypes.ToListAsync();
            if(expenseList == null)
            {
                return new List<ExpenseType>();
            }
            return expenseList;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categoryList = await dbContext.Categories.ToListAsync();
            if (categoryList == null)
            {
                return new List<Category>();
            }
            return categoryList;
        }

        public async Task<List<ModeOfPayment>> GetAllModeOfPayments()
        {
            var modeOfPaymentList = await dbContext.ModeOfPayments.ToListAsync();
            if (modeOfPaymentList == null)
            {
                return new List<ModeOfPayment>();
            }
            return modeOfPaymentList;
        }
    }
}