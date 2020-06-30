using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckedInmembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckedInmembersController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            var result = await _context.Set<Member>().ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }


        [HttpGet("{date}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime date)
        {
            var result = await _context.Set<Member>()
                .Where(x => x.CreatedAt.Date == date.Date)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _context.Set<Member>()
                .Where(x => x.CreatedAt.Date >= dateFrom.Date && x.CreatedAt.Date <= dateTo.Date)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpPost("{signIn}/{id}/{date}")]
        public async Task<IActionResult> SigIn([FromBody] SignInOutDTO data)
        {
            if (data.Id <= 0) { return BadRequest(); }

            var result = await _context.Set<Booking>()
                .Where(x => x.Id == data.Id
                    && x.Date.Date == data.date.Date
                    && x.ServiceId == data.ServiceId
                    && x.Time == data.Time)
                .FirstOrDefaultAsync();

            if (result is null) { return BadRequest(); }

            result.SignIn = DateTime.Now;

            _context.Update(result);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{signOut}/{id}")]
        public async Task<IActionResult> SignOut([FromBody] SignInOutDTO data)
        {
            if (data.Id <= 0) { return BadRequest(); }

            var result = await _context.Set<Booking>()
                .Where(x => x.Id == data.Id
                    && x.Date.Date == data.date.Date
                    && x.ServiceId == data.ServiceId
                    && x.Time == data.Time)
                .FirstOrDefaultAsync();

            if (result is null) { return BadRequest(); }

            result.SignOut = DateTime.Now;

            _context.Update(result);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private List<CheckedInMemberDTO> ParseToCheckedInMemeberDTO(List<Member> members)
        {
            var checkedInMembers = new List<CheckedInMemberDTO>();

            foreach (var member in members)
            {
                checkedInMembers.Add(
                    new CheckedInMemberDTO
                    {
                        Name = member.Name,
                        Surname = member.Surname,
                        Mobile = member.Mobile,
                        CheckedInAt = member.CreatedAt
                    });
            }

            return checkedInMembers;
        }
    }
}
