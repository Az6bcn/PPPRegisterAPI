using System;
using System.Threading.Tasks;
using CheckinPPP.DTOs;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Identity;

namespace CheckinPPP.Business
{
    public interface IAccountBusiness
    {
        Task<ApplicationUser> FindUserAsync(string email);
        Task<string> GenerateJwtToken(LoginDTO loginModel);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<UsersAndLinkedUsersDTO> GetUserAndLinkedUsers(Guid id);
        Task<IdentityResult> Login(LoginDTO loginModel);
        Task<IdentityResult> Register(RegisterDTO registerModel);
        Task<IdentityResult> ResetPasswordAsync(string newPassword, string token, ApplicationUser user);
        Task<bool> ValidatePasswordResetTokenAsync(ApplicationUser user, string token);
    }
}