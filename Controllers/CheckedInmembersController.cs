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
            var result = await _context.Set<Booking>()
                .Include(x => x.Member)
                .Where(x => x.MemberId != null
                    && x.SignIn != null)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }


        [HttpGet("{date}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime date)
        {
            var result = await _context.Set<Booking>()
                .Include(x => x.Member)
                .Where(x => x.Date.Date == date.Date
                    && x.MemberId != null)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpGet("{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _context.Set<Booking>()
                 .Include(x => x.Member)
                .Where(x => x.Date.Date >= dateFrom.Date
                    && x.Date.Date <= dateTo.Date
                    && x.MemberId != null)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpPost("signIn/{id}")]
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

            return Ok(MapToSignInOutResponseDTO(result, true));
        }

        [HttpPost("signOut/{id}")]
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

            return Ok(MapToSignInOutResponseDTO(result));
        }

        private List<CheckedInMemberDTO> ParseToCheckedInMemeberDTO(List<Booking> bookings)
        {
            var checkedInMembers = new List<CheckedInMemberDTO>();

            foreach (var booking in bookings)
            {
                checkedInMembers.Add(
                    new CheckedInMemberDTO
                    {
                        Id = booking.Id,
                        Name = booking.Member.Name,
                        Surname = booking.Member.Surname,
                        Mobile = booking.Member.Mobile,
                        ServiceId = booking.ServiceId,
                        SignedIn = booking.SignIn,
                        SignedOut = booking.SignOut,
                        Date = booking.Date,
                        Time = booking.Time
                    });
            }
            return checkedInMembers;
        }


        private SignInOutResponseDTO MapToSignInOutResponseDTO(Booking booking, bool isSignIn = false)
        {
            SignInOutResponseDTO response;

            if (isSignIn)
            {
                response = new SignInOutResponseDTO
                {
                    Id = booking.Id,
                    Date = (DateTime)booking.SignIn
                };

                return response;
            }

            response = new SignInOutResponseDTO
            {
                Id = booking.Id,
                Date = (DateTime)booking.SignOut
            };

            return response;
        }
    }
}
