using Azure;
using Azure.Communication.Email;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Core.Interfaces;

namespace expenseTrackerPOC.Services.Core
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient emailClient;
        public EmailService(string emailAzureConnectionString) 
        {
            emailClient = new EmailClient(emailAzureConnectionString);
        }

        public async Task<(bool Success, string Message)> SendForgotPasswordOtpMail(string email, int? otpValue, int? expirySec)
        {
            var subject = "Forgot Password : OTP";
            var htmlContent =
                "<html><body>" +
                $"<h1> OTP : {otpValue} </h1>" +
                "<br/>" +
                $"<h4>Hey,</h4>" +
                "<p>Seems like yu forgot your password for expenseTracker.</p>" +
                "<p> Use above otp to verify.</p>" +
                $"<p> The OTP in only valid for next {expirySec} secs.</p>" +
                "</body></html>";
            var sender = "DoNotReply@b6363c99-dbf3-4c3d-932f-2b4ef52ffd86.azurecomm.net";
            var recipient = email;

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(Azure.WaitUntil.Started, sender, recipient, subject, htmlContent);

            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    return (true, $"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                }
            }
            catch (RequestFailedException ex)
            {
                return (false, $"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
            }

            return (true, "Email Sent successfully!");
        }

        public async Task<(bool Success, string Message)> SendLoginAttemptMail(UserDto user)
        {
            var subject = "Security Alert: New Login Attempt Detected";
            var htmlContent =
                "<html><body>" +
                "<h1> New Login Attempt Detected </h1>" +
                "<br/>" +
                $"<h4>Dear {user.UserName},</h4>" +
                "<p>We detected a recent login attempt to your account.</p>" +
                "<p> If this was you, no further action is required. </p>" +
                "</body></html>";
            var sender = "DoNotReply@b6363c99-dbf3-4c3d-932f-2b4ef52ffd86.azurecomm.net";
            var recipient = "aalokchoudhari2021@gmail.com";

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(Azure.WaitUntil.Started,sender,recipient,subject,htmlContent);

            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    return(true, $"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                }
            }
            catch (RequestFailedException ex)
            {
                return(false,$"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
            }

            return (true, "Email Sent successfully!");
        }

        public async Task<(bool Success, string Message)> SendPasswordUpdateMail(string email)
        {
            var subject = "New Password Detected";
            var htmlContent =
                "<html><body>" +
                "<h1> Password Updated Successfully! </h1>" +
                "<br/>" +
                $"<p>Password was updated for {email} to your account.</p>" +
                "<p> If this was you, no further action is required. </p>" +
                "</body></html>";
            var sender = "DoNotReply@b6363c99-dbf3-4c3d-932f-2b4ef52ffd86.azurecomm.net";
            var recipient = $"{email}";

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(Azure.WaitUntil.Started, sender, recipient, subject, htmlContent);

            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    return (true, $"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                }
            }
            catch (RequestFailedException ex)
            {
                return (false, $"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
            }

            return (true, "Email Sent successfully!");
        }

        public async Task<(bool Success, string Message)> SendWelcomeMail(UserDto user)
        {
            var subject = $"Welcome {user.UserName}";
            var htmlContent =
                "<html><body>" +
                $"<h1>Dear {user.UserName},</h1>" +
                "<br/>" +
                "<h4>Welcome to expenseTracker</h4>" +
                "<p>The expenseTracker helps by tracking your expense/transactions for you all at one place.</p>" +
                "<p> With Gratitude</p>" +
                "<p> Team @expenseTracker</p>" +
                "</body></html>";
            var sender = "DoNotReply@b6363c99-dbf3-4c3d-932f-2b4ef52ffd86.azurecomm.net";
            var recipient = $"{user.Email}";

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(Azure.WaitUntil.Started, sender, recipient, subject, htmlContent);

            try
            {
                while (true)
                {
                    await emailSendOperation.UpdateStatusAsync();
                    if (emailSendOperation.HasCompleted)
                    {
                        break;
                    }
                    await Task.Delay(100);
                }

                if (emailSendOperation.HasValue)
                {
                    return (true, $"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                }
            }
            catch (RequestFailedException ex)
            {
                return (false, $"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
            }

            return (true, "Email Sent successfully!");
        }
    }
}
