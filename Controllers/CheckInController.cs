using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CheckInController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckIn data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var member = ParseToMember(data);

            _context.Add<Member>(member);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok();
            }

            return BadRequest();
        }

        private Member ParseToMember(CheckIn checkinData)
        {
            var member = new Member
            {
                Name = checkinData.Name,
                Surname = checkinData.Surname,
                Mobile = checkinData.Mobile
            };

            return member;
        }
    }
}
