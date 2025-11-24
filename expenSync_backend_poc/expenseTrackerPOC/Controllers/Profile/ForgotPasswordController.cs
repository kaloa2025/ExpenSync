using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Profile.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace expenseTrackerPOC.Controllers.Profile
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private IEmailService emailService;
        private IProfileService profileService;

        public ForgotPasswordController(IEmailService emailService, IProfileService profileService)
        {
            this.emailService = emailService;
            this.profileService = profileService;
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<ForgotPasswordResponse>> RequestForgotPasswordReset([FromBody] ForgotPasswordRequest forgotPasswordRequest )
        {
            //1.valid email
            if (!ModelState.IsValid)
            {
                return BadRequest(new ForgotPasswordResponse
                {
                    Success = false,
                    Message = "Invalid Input, Please enter valid email",
                    OtpExpirySec = null
                });
            }
            //2.check if email exists in db
            var validEmail = await profileService.CheckEmailExistsAsync(forgotPasswordRequest.Email);
            if (!validEmail.exists)
            {
                return new ForgotPasswordResponse
                {
                    Success = false,
                    Message = validEmail.Message,
                    OtpExpirySec = null
                };
            }
            //3.create otp and save in db
            var otp = await profileService.CreateOtpForEmail(forgotPasswordRequest.Email);
            if (otp.otpValue == null || otp.expirySec == null || !otp.Success)
            {
                return new ForgotPasswordResponse
                {
                    Success = false,
                    Message = otp.Message,
                    OtpExpirySec = null
                };
            }
            //4.send email with curated otp
            var emailProccessed = await emailService.SendForgotPasswordOtpMail(forgotPasswordRequest.Email, otp.otpValue, otp.expirySec);
            if (!emailProccessed.Success)
            {
                return new ForgotPasswordResponse
                {
                    Success = false,
                    Message = emailProccessed.Message,
                    OtpExpirySec = null
                };
            }
            //5.send success
            return Ok(new ForgotPasswordResponse
            {
                Success = true,
                Message = "An OTP is sent to registered email successfully!",
                Email = forgotPasswordRequest.Email,
                OtpExpirySec = otp.expirySec,
                Errors = new List<string>()
            });
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<VerifyOtpResponse>> VerifyOtp([FromBody] VerifyOtpRequest verifyOtpRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new VerifyOtpResponse
                {
                    Success = false,
                    Message = "Invalid Otp",
                    Errors = new List<string>()
                });
            }
            var verification = await profileService.VerifyOtp(verifyOtpRequest);
            if (!verification.Success)
            {
                return new VerifyOtpResponse
                {
                    Success = verification.Success,
                    Message = verification.Message,
                    Errors = new List<string>()
                };
            }

            return Ok(new VerifyOtpResponse
            {
                Success = verification.Success,
                Message = verification.Message,
                Errors = new List<string>(),
                Email = verifyOtpRequest.Email
            });
        } 

        [HttpPost("resend-otp")]
        public async Task<ActionResult<ForgotPasswordResponse>> ResendOtp([FromBody] ForgotPasswordRequest resendOtpRequest)
        {
            return await RequestForgotPasswordReset(resendOtpRequest);
        }

        [HttpPut("reset-new-password")]
        public async Task<ActionResult<ResetNewPasswordResponse>> ResetNewPassword([FromBody] ResetNewPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResetNewPasswordResponse
                {
                    Success = false,
                    Message = "Invalid Passwords",
                    Errors = new List<string>()
                });
            }

            var passwordUpdate = await profileService.ResetUserPassword(resetPasswordRequest);

            if (!passwordUpdate.Success)
            {
                return new ResetNewPasswordResponse
                {
                    Success = false,
                    Message = passwordUpdate.Message,
                    Errors = new List<string>()
                };
            }

            var informUser = await emailService.SendPasswordUpdateMail(resetPasswordRequest.Email);
            if (!informUser.Success)
            {
                return new ResetNewPasswordResponse
                {
                    Success = false,
                    Message = "New Password updated but couldn't be communicated for now.",
                    Errors = new List<string>()
                };
            }


            return new ResetNewPasswordResponse
            {
                Success = true,
                Message = passwordUpdate.Message,
                Errors = new List<string>()
            };
        }
    }
}