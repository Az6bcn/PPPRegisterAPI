using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.DTOs;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Identity;

namespace CheckinPPP.Business
{
    public class AccountBusiness : IAccountBusiness
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly ApplicationDbContext _dbContext;

        public AccountBusiness(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtFactory jwtFactory,
            ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _dbContext = dbContext;
        }


        /// <summary>
        /// Register to use our Application
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> Register(RegisterDTO registerModel)
        {
            var appUser = ParseToAppUser(registerModel);
            var result = await _userManager.CreateAsync(appUser, registerModel.Password);

            if (result.Succeeded)
            {
                var registeredUser = await _userManager.FindByEmailAsync(registerModel.Email);
                // add the users claims
                var res = await AddUserClaims(registeredUser);

                if (!res.Succeeded) { return IdentityResult.Failed(new IdentityError { Description = "User cliams not added" }); }
            }

            return result;
        }


        /// <summary>
        /// Login in the user, checks if the users email has been confirmed first and if the user exists in our system.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> Login(LoginDTO loginModel)
        {
            // check user exists in our database
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user is null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Please register and confirm your email before log in." });
            }

            var result = await SignInUser(user, loginModel);

            return result.Succeeded ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"{result.ToString()}" });
        }

        public async Task<UsersAndLinkedUsersDTO> GetUserAndLinkedUsers(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            var assignedUsers = _dbContext.Users
                .Where(x => x.Email == user.Email
                    && x.Id != user.Id)
                .ToList();

            // convert ot members
            var response = MapToLinkedUsersDTOs(user, assignedUsers);

            return response;
        }

        /// <summary>
        /// Generate token for successully loggedIn user
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public async Task<string> GenerateJwtToken(LoginDTO loginModel)
        {
            // get user
            var appUser = await _userManager.FindByEmailAsync(loginModel.Email);

            // get user claimss
            var claims = await GetUserClaims(appUser);


            // generate token
            var jwt = _jwtFactory.GenerateToken(claims);

            return jwt;
        }

        public async Task AddUsersAsync(IEnumerable<MemberDTO> members)
        {
            var errors = new List<IdentityResult>();

            var users = MapToApplicationUser(members);

            // add the users
            foreach (var user in users)
            {
                await _userManager.CreateAsync(user);
            };
        }

        private ApplicationUser ParseToAppUser(RegisterDTO register)
        {
            return new ApplicationUser
            {
                Name = register.Name,
                Email = register.Email,
                CreatedAt = DateTime.Now,
                Surname = register.Surname,
                Gender = register.Gender,
                UserName = register.Email,
                PhoneNumber = register.Mobile
            };
        }

        private async Task<List<Claim>> GetUserClaims(ApplicationUser appUser)
        {
            var claims = await _userManager.GetClaimsAsync(appUser);
            var claimsList = claims.ToList();

            var validClaims = claimsList;


            return validClaims;
        }

        private async Task<IdentityResult> AddUserClaims(ApplicationUser appUser)
        {
            var claimsList = new List<Claim>();

            claimsList.Add(new Claim("Id", appUser.Id));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Email, appUser.Email));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Gender, appUser.Gender));

            var result = await _userManager.AddClaimsAsync(appUser, claimsList);
            return result;

        }

        private async Task<SignInResult> SignInUser(ApplicationUser appUser, LoginDTO loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(appUser, loginModel.Password, false, false);
            return result;
        }

        private UsersAndLinkedUsersDTO MapToLinkedUsersDTOs(ApplicationUser mainUser, IEnumerable<ApplicationUser> linkedUsers)
        {
            var response = new UsersAndLinkedUsersDTO
            {
                MainUser = new MemberDTO
                {
                    Id = mainUser.Id,
                    EmailAddress = mainUser.Email,
                    Name = mainUser.Name,
                    Surname = mainUser.Surname,
                    Gender = mainUser.Gender,
                    CategoryId = mainUser.CategoryId
                },
                LinkedUsers = GetMemberDTOs(linkedUsers)
            };

            return response;
        }

        private List<MemberDTO> GetMemberDTOs(IEnumerable<ApplicationUser> linkedUsers)
        {
            var list = new List<MemberDTO>();

            foreach (var user in linkedUsers)
            {
                list.Add(
                    new MemberDTO
                    {
                        Id = user.Id,
                        EmailAddress = user.Email,
                        Name = user.Name,
                        Surname = user.Surname,
                        Gender = user.Gender,
                        CategoryId = user.CategoryId
                    });
            }

            return list;
        }

        private IEnumerable<ApplicationUser> MapToApplicationUser(IEnumerable<MemberDTO> members)
        {
            var users = new List<ApplicationUser>();

            foreach (var member in members)
            {
                users.Add(
                    new ApplicationUser
                    {
                        Email = member.EmailAddress,
                        CreatedAt = DateTime.Now,
                        Gender = member.Gender,
                        Name = member.Name,
                        Surname = member.Surname,
                        UserName = member.EmailAddress,
                        PhoneNumber = member.Mobile
                    });
            }

            return users;
        }
    }
}
