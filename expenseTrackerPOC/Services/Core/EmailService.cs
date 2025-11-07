using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Services.Core.Interfaces;

namespace expenseTrackerPOC.Services.Core
{
    public class EmailService : IEmailService
    {
        public Task SendLoginAttemptMail(UserDto user)
        {
            return Task.CompletedTask;
        }

        public Task SendWelcomeMail(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
