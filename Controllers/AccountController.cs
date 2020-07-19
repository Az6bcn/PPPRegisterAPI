using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Net.Http.Headers;
using RestSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountBusiness accountBusiness;

        public AccountController(IAccountBusiness accountBusiness)
        {
            this.accountBusiness = accountBusiness;
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
            var token = Request.Headers[HeaderNames.Authorization];
            var response = await accountBusiness.GetUserAndLinkedUsers(id);

            return Ok(response);
        }
    }
}
