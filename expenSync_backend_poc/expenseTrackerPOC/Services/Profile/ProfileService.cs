using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Auth;
using expenseTrackerPOC.Services.Profile.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace expenseTrackerPOC.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly ExpenseTrackerDbContext dbContext;
        public ProfileService(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(bool exists, string Message)> CheckEmailExistsAsync(string email)
        {
            if (email == null)
            {
                return (false, "Email is not valid");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return (false, $"An User with email : {email} doesn't exists.");
            }

            return (true, "User with email exists.");
        }

        public async Task<(bool exists, string Message)> CheckUserExistsAsync(string email, int userId)
        {
            if (email == null)
            {
                return (false, "Email is not valid");
            }
            if (userId == null||userId==0)
            {
                return (false, "UserId is not valid");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserId == userId);

            if (user == null)
            {
                return (false, $"An User with email : {email} doesn't exists.");
            }

            return (true, "User with email exists.");
        }

        public async Task<(bool Success, int? otpValue, int? expirySec, string Message)> CreateOtpForEmail(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u=>u.Email == email);
            if (user == null)
            {
                return (false, null, null, $"No user for {email} found");
            }
            int otp = RandomNumberGenerator.GetInt32(1000, 10000);
            var expirySec = 120;

            user.PasswordResetExpiry = DateTime.UtcNow.AddSeconds(expirySec);
            user.PasswordResetOtp = otp;
            user.IsOtpVerified = false;

            try
            {
                await dbContext.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                return (false, null, null, $"Cound't Finish Otp Creation Process { ex.Message}");
            }

            return (true, otp, expirySec, "OTP created successfully!");
        }

        public async Task<(UserDto? user, string Message)> EditUserDetailsAsync(EditProfileRequest editProfileRequest, int userId)
        {
            if (editProfileRequest == null)
            {
                return (null, "Data is invalid, try again later!");
            }

            // 1. Find the existing user
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingUser == null)
            {
                return (null, "User not found!");
            }

            // 2. Update user details
            existingUser.Email = editProfileRequest.Email;
            existingUser.Username = editProfileRequest.UserName;

            // 3. Save changes
            dbContext.Users.Update(existingUser);
            await dbContext.SaveChangesAsync();

            // 4. Return updated user info
            var userDto = new UserDto
            {
                Id = existingUser.UserId,
                Email = existingUser.Email,
                UserName = existingUser.Username,
                Role = existingUser.Role
            };

            return (userDto, "User details updated successfully!");
        }

        public async Task<(bool Success, string Message)> ResetUserPassword(ResetNewPasswordRequest resetPasswordRequest)
        {
            if (resetPasswordRequest == null)
            {
                return (false, "Invalid Input");
            }
            
            var user = await dbContext.Users.FirstOrDefaultAsync(u=>u.Email == resetPasswordRequest.Email);
            if (user == null)
            {
                return (false, $"Couldn't Find user for this {resetPasswordRequest.Email}");
            }

            if(!resetPasswordRequest.Password.Equals(resetPasswordRequest.ConfirmPassword))
            {
                return (false, $"Passwords {resetPasswordRequest.Password} and {resetPasswordRequest.ConfirmPassword} aren't same.");
            }

            if(!user.IsOtpVerified)
            {
                return (false, $"OTP is not verified.");
            }
            if (user.PasswordResetExpiry < DateTime.UtcNow)
            {
                user.IsOtpVerified = false;
                await dbContext.SaveChangesAsync();
                return (false, "OTP verification expired. Please verify again.");
            }

            user.HashPassword = PasswordHasher.Hash(resetPasswordRequest.Password);
            user.IsOtpVerified = false;
            await dbContext.SaveChangesAsync();

            return (true, "Password updated Successfully!");

        }

        public async Task<(bool Success, string Message)> VerifyOtp(VerifyOtpRequest verifyOtpRequest)
        {
            if(verifyOtpRequest == null)
            {
                return (false, "Enter correct Otp");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u=>u.Email == verifyOtpRequest.Email);
            if(user == null)
            {
                return (false, $"Couldn't Find user for this {verifyOtpRequest.Email}");
            }
            if (user.PasswordResetExpiry < DateTime.UtcNow)
            {
                return (false, "Expired OTP");
            }
            if(user.PasswordResetOtp != verifyOtpRequest.Otp)
            {
                return (false, "Invalid OTP");
            }

            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(1);
            user.PasswordResetOtp = null;
            user.IsOtpVerified = true;

            await dbContext.SaveChangesAsync();

            return (true, "OTP verfied Successfully");
        }
    }
}
