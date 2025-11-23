using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Services.Auth;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace expenseTrackerPOC.Controllers.Profile
{
    [Route("api/profile")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private IEmailService emailService;
        private IJwtService jwtService;
        private IProfileService profileService;

        public ProfileController(IJwtService jwtService, IEmailService emailService, IProfileService profileService)
        {
            this.jwtService = jwtService;
            this.emailService = emailService;
            this.profileService = profileService;
        }

        [HttpPut("update")]
        public async Task<ActionResult<EditProfileResponse>> EditProfile([FromBody] EditProfileRequest editProfileRequest)
        {
            //1. Validate Model
            if (!ModelState.IsValid)
            {
                return BadRequest(new EditProfileResponse
                {
                    Success = false,
                    Message = "Invalid Input Data",
                    Errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new EditProfileResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            //2. Check User Exists
            var userExists = await profileService.CheckUserExistsAsync(editProfileRequest.Email, userId);
            if (!userExists.exists)
            {
                return StatusCode(403, new EditProfileResponse
                {
                    Success = false,
                    Message = userExists.Message,
                });
            }

            //3. Edit User
            var updatedUser = await profileService.EditUserDetailsAsync(editProfileRequest, userId);
            if (updatedUser.user == null)
            {
                return Unauthorized(new EditProfileResponse
                {
                    Success = false,
                    Message = updatedUser.Message,
                });
            }

            //4. Return
            return Ok(new EditProfileResponse
            {
                Success = true,
                User = updatedUser.user,
                Message = "User details updated Successfully!"
            });
        }
    }
}

