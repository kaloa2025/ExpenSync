using expenseTrackerPOC.Data;
using expenseTrackerPOC.Services.Core.Interfaces;

namespace expenseTrackerPOC.Services.Core
{
    public class CoreService : ICoreService
    {
        private readonly ExpenseTrackerDbContext dbContext;
        public CoreService(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
