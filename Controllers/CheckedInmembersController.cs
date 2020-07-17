using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
                .Include(x => x.User)
                .Where(x => x.UserId != null
                    && x.SignIn != null)
                .OrderBy(x => x.User.Surname)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }


        [HttpGet("{date}/{serviceId}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime date, int serviceId)
        {
            if (serviceId == 0 || !isValidServiceId(serviceId)) { return BadRequest(); }

            var result = await _context.Set<Booking>()
                .Include(x => x.User)
                .Where(x => x.Date.Date == date.Date
                    && x.UserId != null
                    && x.ServiceId == serviceId)
                .OrderBy(x => x.User.Surname)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpGet("{dateFrom}/{dateTo}/{serviceId}")]
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime dateFrom, DateTime dateTo, int serviceId)
        {
            if (serviceId == 0 || !isValidServiceId(serviceId)) { return BadRequest(); }

            var result = await _context.Set<Booking>()
                 .Include(x => x.User)
                .Where(x => x.Date.Date >= dateFrom.Date
                    && x.Date.Date <= dateTo.Date
                    && x.UserId != null
                     && x.ServiceId == serviceId)
                .OrderBy(x => x.User.Surname)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(result);

            return Ok(mappedResult);
        }

        [HttpGet("pickUpReport/{date}")]
        public async Task<IActionResult> GetAllPickUpRecordsOnSpecifiedDate(DateTime date)
        {
            var result = await _context.Set<Booking>()
                .Include(x => x.User)
                .Where(x => x.Date.Date == date.Date
                    && x.UserId != null
                    && x.PickUp)
                .OrderBy(x => x.User.Surname)
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

        [HttpGet("services")]
        public IActionResult GetServices()
        {
            var services = GetAllServices();

            return Ok(services);
        }

        [HttpGet("attendance/{selectedDate}")]
        public async Task<IActionResult> GetAttendanceAsync(DateTime selectedDate)
        {
            var date = selectedDate;

            //var date = DateTime.Parse("2020-07-05 00:00:00.0000000");

            var result = await _context.Set<Booking>()
                .Where(x => x.Date == date.Date)
                .ToListAsync();

            var groupedResult = result.GroupBy(x => x.ServiceId)
                .Select(x => new
                {
                    ServiceId = x.Key,
                    Total = x.Select(y => y.ServiceId == x.Key).Count(),
                    Attended = x.Where(y => y.ServiceId == x.Key
                        && y.SignIn != null).Count()

                })
                .ToList();

            var total = new
            {
                TotalSlots = result.Count(),
                TotalSlotsBooked = result.Where(x => x.BookingReference != null).Count(),
                TotalAttended = result.Where(x => x.SignIn != null).Count()
            };

            var response = new
            {
                groupedResult = groupedResult,
                total = total
            };

            return Ok(response);

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
                        Name = booking.User.Name,
                        Surname = booking.User.Surname,
                        Mobile = booking.User.PhoneNumber,
                        ServiceId = booking.ServiceId,
                        SignedIn = booking.SignIn,
                        SignedOut = booking.SignOut,
                        Date = booking.Date,
                        Time = booking.Time,
                        PickUp = booking.PickUp,
                        Gender = booking.User.Gender
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


        private bool isValidServiceId(int serviceId)
        {
            var response = GetAllServices()
                .Select(x => x.Id)
                .Contains(serviceId);

            return response;
        }

        private IEnumerable<ServiceDTO> GetAllServices()
        {
            var services = new List<ServiceDTO>
            {
                new ServiceDTO{Id = 1, Name = "Second Service" },
                new ServiceDTO{Id = 2, Name = "Third Service" },
                new ServiceDTO{Id = 3, Name = "First Service" }
            };

            return services;
        }
    }
}
