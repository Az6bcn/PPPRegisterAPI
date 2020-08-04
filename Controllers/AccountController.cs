using System;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.DTOs;
using CheckinPPP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountBusiness accountBusiness;
        private readonly IGoogleMailService _googleMailService;

        public AccountController(
            IAccountBusiness accountBusiness,
            IGoogleMailService googleMailService
           )
        {
            this.accountBusiness = accountBusiness;
            _googleMailService = googleMailService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await accountBusiness.Register(registerModel);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(errors);
            }

            return Ok(new { success = true });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var result = await accountBusiness.Login(loginModel);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(errors);
            }

            // generte token
            var token = await accountBusiness.GenerateJwtToken(loginModel);

            return Ok(new { accesToken = token });
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserAndLinkedUsers(Guid id)
        {
            var response = await accountBusiness.GetUserAndLinkedUsers(id);

            return Ok(response);
        }

        [HttpGet("passwordresettoken/{email}")]
        public async Task<IActionResult> GeneratePasswordResetToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Can not request password reset for invalid email");
            }

            var user = await accountBusiness.FindUserAsync(email);

            if (user is null)
            {
                return BadRequest("Can not request password reset.");
            }

            var token = await accountBusiness.GeneratePasswordResetTokenAsync(user);

            // generate url with token
            //var url = Url.Action("ResetPassword", "Account", new { token = token, email = email }, Request.Scheme);

            // send via email
            await _googleMailService.SendPasswordResetEmailAsync(email, user, token);

            return Ok();
        }

        [HttpPost("passwordreset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordModel)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordModel.Token) || string.IsNullOrWhiteSpace(resetPasswordModel.Email))
            {
                return BadRequest("Can not reset password, invalid token or email");
            }

            var user = await accountBusiness.FindUserAsync(resetPasswordModel.Email);

            if (user is null)
            {
                return BadRequest("Can not reset password for invalid user.");
            }

            // reset password
            var response = await accountBusiness.ResetPasswordAsync(resetPasswordModel.NewPassword, resetPasswordModel.Token, user);

            if (!response.Succeeded)
            {
                var errors = response.Errors.Select(x => x.Description).ToList();

                return BadRequest(errors);
            }

            return Ok();
        }
    }
}
