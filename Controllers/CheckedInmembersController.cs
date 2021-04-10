using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using CheckinPPP.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckedInmembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingQueries _bookingQueries;

        public CheckedInmembersController(
            ApplicationDbContext context,
            IHubContext<PreciousPeopleHub,
            IPreciousPeopleClient> hubContext,
            IBookingQueries bookingQueries)
        {
            _context = context;
            _bookingQueries = bookingQueries;
            _hubContext = hubContext;
        }

        private IHubContext<PreciousPeopleHub, IPreciousPeopleClient> _hubContext { get; }


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
            if (serviceId == 0 || !isValidServiceId(serviceId)) return BadRequest();

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
        public async Task<IActionResult> GetAllRecordsUpToSpecifiedDate(DateTime dateFrom, DateTime dateTo,
            int serviceId)
        {
            if (serviceId == 0 || !isValidServiceId(serviceId)) return BadRequest();

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
            if (data.Id <= 0) return BadRequest();

            var result = await _context.Set<Booking>()
                .Where(x => x.Id == data.Id
                            && x.Date.Date == data.date.Date
                            && x.ServiceId == data.ServiceId
                            && x.Time == data.Time)
                .FirstOrDefaultAsync();

            if (result is null) return BadRequest();

            result.SignIn = DateTime.Now;

            _context.Update(result);

            await _context.SaveChangesAsync();


            var bookings = await _context.Set<Booking>()
                .Include(x => x.User)
                .Where(x => x.Date.Date == data.date.Date
                            && x.UserId != null
                            && x.ServiceId == data.ServiceId)
                .OrderBy(x => x.User.Surname)
                .ToListAsync();

            var mappedResult = ParseToCheckedInMemeberDTO(bookings);

            await _hubContext.Clients.All.ReceivedBookingsToSignInUpdateAsync(mappedResult);

            return Ok(MapToSignInOutResponseDTO(result, true));
        }

        [HttpPost("signOut/{id}")]
        public async Task<IActionResult> SignOut([FromBody] SignInOutDTO data)
        {
            if (data.Id <= 0) return BadRequest();

            var result = await _context.Set<Booking>()
                .Where(x => x.Id == data.Id
                            && x.Date.Date == data.date.Date
                            && x.ServiceId == data.ServiceId
                            && x.Time == data.Time)
                .FirstOrDefaultAsync();

            if (result is null) return BadRequest();

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

            var isSpecialService = result?.FirstOrDefault()?.IsSpecialService;

            if (isSpecialService.HasValue && isSpecialService.Value)
            {
                result = result.Where(x => x.ShowSpecialService).ToList();
            }
            else
            {
                result = result.Where(x => x.ShowSundayService).ToList();
            }

            var groupedResult = result.GroupBy(x => x.ServiceId)
                .Select(x => new
                {
                    ServiceId = x.Key,
                    Total = x.Select(y => y.ServiceId == x.Key).Count(),
                    Attended = x.Count(y => y.ServiceId == x.Key
                                            && y.SignIn != null)
                })
                .ToList();

            var total = new
            {
                TotalSlots = result.Count(),
                TotalSlotsBooked = result.Count(x => x.BookingReference != null),
                TotalAttended = result.Count(x => x.SignIn != null)
            };

            var response = new
            {
                groupedResult,
                total
            };

            return Ok(response);
        }

        [HttpGet("slots/resize/{numberSlots}")]
        public async Task<IActionResult> ResizeSlot(int numberSlots)
        {
            var date = DateTime.UtcNow;
            var nextSunday = date.AddDays(8).Date;

            if (nextSunday.Month != DateTime.UtcNow.Month)
            {
                // new month's 1st sunday: Thanksgiving
                var thanksgivingResponse = new SlotResizeResponseDto()
                {
                    Resized = false,
                    IsThanksgivingSunday = true
                };

                return Ok(thanksgivingResponse);
            }

            var times = new List<string> { "08:30", "10:10", "11:50" };
            var bookingsToMakeUnavailable =
                await _bookingQueries.GetBookingsForDateByTime(nextSunday, times, numberSlots);

            bookingsToMakeUnavailable.ToList().ForEach(booking => booking.ShowSundayService = false);

            _context.UpdateRange(bookingsToMakeUnavailable);
            var updatedCount = await _context.SaveChangesAsync();

            var response = updatedCount > 0
                ? new SlotResizeResponseDto()
                {
                    Resized = true,
                    IsThanksgivingSunday = false
                }
                : new SlotResizeResponseDto()
                {
                    Resized = false,
                    IsThanksgivingSunday = false
                };

            return Ok(response);
        }

        private List<CheckedInMemberDTO> ParseToCheckedInMemeberDTO(List<Booking> bookings)
        {
            var checkedInMembers = new List<CheckedInMemberDTO>();

            foreach (var booking in bookings)
                checkedInMembers.Add(
                    new CheckedInMemberDTO
                    {
                        Id = (int)booking?.Id,
                        Name = booking?.User?.Name,
                        Surname = booking?.User?.Surname,
                        Mobile = booking?.User?.PhoneNumber,
                        ServiceId = (int)booking?.ServiceId,
                        SignedIn = booking?.SignIn,
                        SignedOut = booking?.SignOut,
                        Date = (DateTime)booking?.Date,
                        Time = booking?.Time,
                        PickUp = (bool)booking?.PickUp,
                        Gender = booking?.User?.Gender
                    });

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
                new ServiceDTO {Id = 1, Name = "Second Service"},
                new ServiceDTO {Id = 2, Name = "Third Service"},
                new ServiceDTO {Id = 3, Name = "First Service"},
                new ServiceDTO {Id = 4, Name = "Special Service"}
            };

            return services;
        }
    }
}