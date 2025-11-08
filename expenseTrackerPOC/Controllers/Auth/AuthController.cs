using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

                //4. Save RefreshToken in db
                //await authService.SaveRefreshTokenAsync(refreshToken);
                //await authService.RemoveOldRefreshTokensAsync(user.Id);

                //5. Mail User about Login Attempt
                //await emailService.SendLoginAttemptMail(user);

                //6. Return
                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = token,
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


        [HttpPost("signup")]
        public async Task<ActionResult<SignUpResponse>> Register([FromBody] SignUpRequest signUpRequest)
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

                //2. Check User Exists
                var userExists = await authService.CheckUserEmailAlreadyExistsAsync(signUpRequest.Email);
                if(userExists)
                {
                    return StatusCode(409, new SignUpResponse
                    {
                        Success = false,
                        Message = "An User with same email exists, Please Login if you are a returning User.",
                    });
                }

                //2. Add User
                var user = await authService.CreateUserAsync(signUpRequest);
                if (user == null)
                {
                    return Unauthorized(new SignUpResponse
                    {
                        Success = false,
                        Message = "Unable to create new User, Please Try again after some time."
                    });
                }

                //3. Generate JWT
                var (token, refreshToken) = await jwtService.GenerateTokenAsync(user);

                //4. Save RefreshToken in db
                //await authService.SaveRefreshTokenAsync(refreshToken);
                //await authService.RemoveOldRefreshTokensAsync(user.Id);

                //5. Send Welcome Mail
                //await emailService.SendWelcomeMail(user);

                //6. Return
                return Ok(new SignUpResponse
                {
                    Success = true,
                    Token = token,
                    User = user,
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new SignUpResponse
                {
                    Success = false,
                    Message = "An error Occured during SignUp",
                    Errors = new List<string> { ex.Message }
                });
            }
        }




        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken()
        {
            try
            {
                // 1. Get refresh token from cookie
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new RefreshTokenResponse
                    {
                        Success = false,
                        Message = "Refresh token is required"
                    });
                }

                // 2. Get user by refresh token
                var user = await authService.GetUserByRefreshTokenAsync(refreshToken);
                if (user == null)
                {
                    return Unauthorized(new RefreshTokenResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    });
                }

                // 3. Get the refresh token entity
                var oldRefreshToken = await authService.GetRefreshTokenAsync(refreshToken);
                if (oldRefreshToken == null || !oldRefreshToken.IsActive)
                {
                    return Unauthorized(new RefreshTokenResponse
                    {
                        Success = false,
                        Message = "Invalid refresh token"
                    });
                }

                // 4. Generate new tokens
                var (newAccessToken, newRefreshToken) = await jwtService.GenerateTokenAsync(user);

                // 5. Revoke old refresh token and save new one
                await authService.RevokeRefreshTokenAsync(oldRefreshToken, "Replaced by new token");
                newRefreshToken.ReplacedByToken = newRefreshToken.Token;
                await authService.SaveRefreshTokenAsync(newRefreshToken);
                await authService.RemoveOldRefreshTokensAsync(user.Id);

                // 6. Set new refresh token in cookie
                SetRefreshTokenCookie(newRefreshToken.Token);

                return Ok(new RefreshTokenResponse
                {
                    Success = true,
                    AccessToken = newAccessToken,
                    Message = "Token refreshed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RefreshTokenResponse
                {
                    Success = false,
                    Message = "An error occurred during token refresh",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                var token = await authService.GetRefreshTokenAsync(refreshToken);
                if (token == null || !token.IsActive)
                {
                    return BadRequest(new { message = "Invalid refresh token" });
                }

                await authService.RevokeRefreshTokenAsync(token, "Revoked by user");

                // Clear the cookie
                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var token = await authService.GetRefreshTokenAsync(refreshToken);
                    if (token != null && token.IsActive)
                    {
                        await authService.RevokeRefreshTokenAsync(token, "User logout");
                    }
                }

                // Clear the cookie
                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(30), // Match your refresh token expiry
                Secure = true, // Set to true in production (HTTPS)
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}