using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Services.Core.Interfaces
{
    public interface ITransactionService
    {
        public Task<AddNewExpenseResponse> AddNewExepense(int userId, AddNewExpenseRequest addNewExpenseRequest);
        public Task<DeleteExpenseResponse> DeleteExepense(int userId, int transactionId);
        public Task<EditExpenseResponse> EditExpense(int userId, int transactionId, EditExpenseRequest editExpenseRequest);
        public Task<GetGraphDataResponse> GetGraphData(int userId);
        public Task<GetRecentTransactionsResponse> GetRecentTransactions(int userId);
        public Task<GetReportResponse> GetReport(int userId);

        public Task<List<ExpenseType>> GetAllExpenseTypes();
        public Task<List<Category>> GetAllCategories();
        public Task<List<ModeOfPayment>> GetAllModeOfPayments();
    }
}

