using System;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using CheckinPPP.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IBookingBusiness _bookingBusiness;
        private readonly IBookingQueries _bookingQueries;

        private IHubContext<PreciousPeopleHub, IPreciousPeopleClient> _hubContext { get; }

        public BookingController(
            IBookingBusiness bookingBusiness,
            IHubContext<PreciousPeopleHub,
            IPreciousPeopleClient> hubContext,
            IBookingQueries bookingQueries
            )
        {
            _bookingBusiness = bookingBusiness;
            _hubContext = hubContext;
            _bookingQueries = bookingQueries;
        }

        [HttpGet("{serviceId}/{date}/{time}")]
        public async Task<IActionResult> GetAvailableBookings(int serviceId, DateTime date, string time)
        {
            var availableBookings = await _bookingQueries.GetAvailableBookingsAsync(serviceId, date, time);

            var availableBookingsDTO = _bookingBusiness.MapToBookingDTO(availableBookings);

            return Ok(availableBookingsDTO);
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetAvailableBookings(DateTime date)
        {
            var availableBookings = await _bookingQueries.GetAvailableBookingsAsync(date);

            var grouped = availableBookings
                .GroupBy(x => x.ServiceId)
                .Select(x => new SlotDTO
                {
                    ServiceId = x.Key,
                    Time = x.Select(y => y.Time).FirstOrDefault(),
                    AvailableSlots = x.Count()
                });

            return Ok(grouped);

        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] BookingDTO booking)
        {
            if (booking is null) { return BadRequest(); }

            // lock

            // group booking : check we have enough slot left for the total number of bookings
            if (booking.IsGroupBooking)
            {
                var groupBookingResponse = await _bookingBusiness.GroupBookingAsync(booking);

                var bookingsUpdate = await _bookingBusiness.GetBookingsUpdateAsync(booking.ServiceId, booking.Date, booking.Time);
                await _hubContext.Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate);

                return Ok(groupBookingResponse);
            }

            // single booking: check select booking still available or any available
            var singleBookingResponse = await _bookingBusiness.SingleBookingAsync(booking);

            var bookingsUpdate2 = await _bookingBusiness.GetBookingsUpdateAsync(booking.ServiceId, booking.Date, booking.Time);
            await _hubContext.Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate2);

            return Ok(singleBookingResponse);
        }

        // send Email (confirming booking and a cancel button) 

        // cancell booking (called from email cancel button)
    }
}
