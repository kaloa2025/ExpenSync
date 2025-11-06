using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace expenseTrackerPOC.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IEmailService emailService;
        private IJwtService jwtService;
        private IAuthService authService;

        public AuthController(IJwtService jwtService, IEmailService emailService, IAuthService authService)
        {
            this.jwtService = jwtService;
            this.emailService = emailService;
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                //1. Validate Model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid Input Data",
                        Errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                //2. Check Credentials
                var user = await authService.ValidateCredentialsAsync(loginRequest);
                if (user == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid Email or Password!"
                    });
                }

                //3. Generate JWT
                var (token, refreshToken) = await jwtService.GenerateTokenAsync(user);

                //4. Mail User about Login Attempt
                await emailService.SendLoginAttemptMail(user);

                //5. Return
                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    User = user,

                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "An error Occured during Login",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

    }
}
