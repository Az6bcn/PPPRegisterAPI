using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using CheckinPPP.Helpers;
using CheckinPPP.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IBookingBusiness _bookingBusiness;
        private readonly IBookingQueries _bookingQueries;
        private readonly ISendEmails _sendEmails;
        private readonly IGoogleMailService _googleMailService;
        private readonly ApplicationDbContext _context;
        private IHubContext<PreciousPeopleHub, IPreciousPeopleClient> _hubContext { get; }

        public BookingController(
            IBookingBusiness bookingBusiness,
            IHubContext<PreciousPeopleHub,
            IPreciousPeopleClient> hubContext,
            IBookingQueries bookingQueries,
            ISendEmails sendEmails,
            IGoogleMailService googleMailService,
            ApplicationDbContext context
            )
        {
            _bookingBusiness = bookingBusiness;
            _hubContext = hubContext;
            _bookingQueries = bookingQueries;
            _sendEmails = sendEmails;
            _googleMailService = googleMailService;
            _context = context;
        }

        [HttpGet("{serviceId}/{date}/{time}")]
        [Authorize]
        public async Task<IActionResult> GetAvailableBookings(int serviceId, DateTime date, string time)
        {
            var availableBookings = await _bookingQueries.GetAvailableBookingsAsync(serviceId, date, time);

            var availableBookingsDTO = _bookingBusiness.MapToBookingDTOs(availableBookings);

            return Ok(availableBookingsDTO);
        }

        [HttpGet("cancellable/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailableBookings(int bookingId)
        {
            var booking = await _bookingQueries.FindBookingByIdAsync(bookingId);

            if (booking.MemberId is null)
            {
                return Ok(new { cancellable = false });
            }

            var availableBookingsDTO = _bookingBusiness.MapToBookingDTO(booking);

            return Ok(new { cancellable = true, data = availableBookingsDTO });
        }

        [HttpGet("{date}")]
        [Authorize]
        public async Task<IActionResult> GetAvailableBookings(DateTime date)
        {
            var availableBookings = await _bookingQueries.GetAvailableBookingsAsync(date);

            var grouped = availableBookings
                .GroupBy(x => x.ServiceId)
                .Select(x => new SlotDTO
                {
                    ServiceId = x.Key,
                    Time = x.Select(y => y.Time).FirstOrDefault(),
                    AdultsAvailableSlots = x.Where(y => y.IsAdultSlot).Count(),
                    KidsAvailableSlots = x.Where(y => y.IsKidSlot).Count(),
                    ToddlersAvailableSlots = x.Where(y => y.IsToddlerSlot).Count()
                });

            return Ok(grouped);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookAppointment([FromBody] BookingDTO booking)
        {
            if (booking is null) { return BadRequest(); }

            // group booking : check we have enough slot left for the total number of bookings
            if (booking.IsGroupBooking)
            {
                var groupBooking = new List<Booking>();

                using var groupBookingTransaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    groupBooking = await _bookingBusiness.GroupBookingAsync(booking);

                    if (!groupBooking.Any())
                    {
                        return BadRequest();
                    }

                    _context.UpdateRange(groupBooking);

                    var numbersUpdated = await _context.SaveChangesAsync();

                    if (numbersUpdated == (booking.Members.Count * 2))
                    {
                        await groupBookingTransaction.CommitAsync();

                    }
                    else
                    {
                        await groupBookingTransaction.RollbackAsync();
                    }

                    var bookingsUpdate = await _bookingQueries.GetBookingsUpdateAsync(booking.ServiceId, booking.Date, booking.Time);
                    await _hubContext.Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate);


                    // all have same email, just mesaage anyone of them
                    var personToEmail = groupBooking.First();

                    await _googleMailService.SendBookingConfirmationEmailAsync(personToEmail.Member.EmailAddress, personToEmail);
                }
                catch
                {
                    await groupBookingTransaction.RollbackAsync();
                }

                return Ok(_bookingBusiness.MapToBookingDTOs(groupBooking));
            }

            // single booking: check select booking still available or any available
            // try and book

            Booking singleBooking = null;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                singleBooking = await _bookingBusiness.SingleBookingAsync(booking);

                if (singleBooking is null)
                {
                    return BadRequest();
                }

                _context.UpdateRange(singleBooking);

                var numbersUpdated = await _context.SaveChangesAsync();

                if (numbersUpdated == 2)
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                    return BadRequest();
                }

                var bookingsUpdate2 = await _bookingQueries.GetBookingsUpdateAsync(booking.ServiceId, booking.Date, booking.Time);
                await _hubContext.Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate2);

                await _googleMailService.SendBookingConfirmationEmailAsync(singleBooking.Member.EmailAddress, singleBooking);
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return Ok(_bookingBusiness.MapToBookingDTO(singleBooking));
        }


        [HttpPost("{bookingId}/{email}/{name}/{surname}")]
        public async Task<IActionResult> CancelBooking(int bookingId, string email, string name, string surname)
        {
            if (bookingId == 0)
            {
                return BadRequest();
            }

            var isValidBooking = await _bookingBusiness.IsValidBookingAsync(bookingId, email, name, surname);

            if (!isValidBooking)
            {
                return BadRequest();
            }

            var booking = await _bookingQueries.FindBookingByIdAsync(bookingId);

            if (booking.GroupLinkId != null)
            {
                var groupBookings = await _bookingQueries.FindBookingsByGoupLinkIdAsync((Guid)booking.GroupLinkId);
                await CancelInsertions(null, groupBookings);

                foreach (var _booking in groupBookings)
                {
                    _booking.MemberId = null;
                    _booking.BookingReference = null;
                    _booking.GroupLinkId = null;
                }


                await _bookingQueries.CancelBookingsAsync(groupBookings);
            }
            else
            {
                booking.MemberId = null;
                await CancelInsertions(booking);
                await _bookingQueries.CancelBookingAsync(booking);
            }

            var bookingsUpdate2 = await _bookingQueries.GetBookingsUpdateAsync(booking.ServiceId, booking.Date, booking.Time);
            await _hubContext.Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate2);

            return Ok();
        }

        private async Task CancelInsertions(Booking booking = null, IEnumerable<Booking> bookings = null)
        {
            if (bookings != null && bookings.Any())
            {
                // insert into cancel table
                var _cancelledBooking = CancelledBookings(bookings);

                await CancelBookingsInsertionAsync(_cancelledBooking);

                return;
            }

            // insert into cancel table
            var cancelledBooking = CancelledBooking(booking);

            await CancelBookingInsertionAsync(cancelledBooking);
        }

        private CancelledBooking CancelledBooking(Booking booking)
        {
            var cancelledBooking = new CancelledBooking
            {
                BookingId = booking.Id,
                Member = booking.Member,
                CancelledAt = DateTime.Now
            };

            return cancelledBooking;
        }

        private IEnumerable<CancelledBooking> CancelledBookings(IEnumerable<Booking> bookings)
        {
            var cancelledBookings = new List<CancelledBooking>();

            foreach (var booking in bookings)
            {
                cancelledBookings.Add(
                new CancelledBooking
                {
                    BookingId = booking.Id,
                    Member = booking.Member,
                    CancelledAt = DateTime.Now
                });
            }

            return cancelledBookings;
        }


        private async Task CancelBookingInsertionAsync(CancelledBooking cancelledBooking)
        {
            _context.Add<CancelledBooking>(cancelledBooking);

            await _context.SaveChangesAsync();
        }

        private async Task CancelBookingsInsertionAsync(IEnumerable<CancelledBooking> cancelledBookings)
        {
            await _context.AddRangeAsync(cancelledBookings);

            await _context.SaveChangesAsync();
        }

    }
}
