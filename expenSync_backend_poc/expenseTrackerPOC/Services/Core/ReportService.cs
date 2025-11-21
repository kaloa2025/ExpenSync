using expenseTrackerPOC.Data;
using expenseTrackerPOC.Services.Core.Interfaces;

namespace expenseTrackerPOC.Services.Core
{
    public class ReportService : IReportService
    {
        private readonly ExpenseTrackerDbContext dbContext;
        public ReportService(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
