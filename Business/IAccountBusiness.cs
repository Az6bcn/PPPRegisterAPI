using System;
using System.Threading.Tasks;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.Identity;

namespace CheckinPPP.Business
{
    public interface IAccountBusiness
    {
        Task<string> GenerateJwtToken(LoginDTO loginModel);
        Task<UsersAndLinkedUsersDTO> GetUserAndLinkedUsers(Guid id);
        Task<IdentityResult> Login(LoginDTO loginModel);
        Task<IdentityResult> Register(RegisterDTO registerModel);
    }
}
