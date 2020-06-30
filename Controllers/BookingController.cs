using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{serviceId}/{date}/{time}")]
        public async Task<IActionResult> GetAvailableBookings(int serviceId, DateTime date, string time)
        {
            var availableBookings = await _context.Set<Booking>()
                .Where(x => x.ServiceId == serviceId
                    && x.Date.Date == date.Date
                    && x.Time == time
                    && x.MemberId == null)
                .ToListAsync();

            var availableBookingsDTO = MapToBookingDTO(availableBookings);

            return Ok(availableBookingsDTO);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] BookingDTO booking)
        {
            if (booking is null) { return BadRequest(); }

            // lock

            // group booking : check we have enough slot left for the total number of bookings
            if (booking.IsGroupBooking)
            {
                var groupBookingResponse = await GroupBooking(booking);

                return Ok(groupBookingResponse);
            }

            // single booking: check select booking still available or any available

            var singleBookingResponse = await SingleBooking(booking);

            return Ok(singleBookingResponse);
        }


        private async Task<List<Booking>> GroupBooking(BookingDTO booking)
        {
            // try and book
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var response = await _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time)
                .Take(booking.TotalNumberBookings)
                .ToListAsync();

                if (response.Count() != booking.TotalNumberBookings)
                {
                    return new List<Booking>();
                }

                var members = MapToMembers(booking);
                var groupId = Guid.NewGuid();

                foreach (var _booking in response)
                {
                    var i = 0;
                    _booking.Member = members[i];
                    _booking.GroupLinkId = groupId;

                    i++;
                }

                _context.UpdateRange(response);
                var numbersUpdated = await _context.SaveChangesAsync();

                if (numbersUpdated == (booking.TotalNumberBookings * 2))
                {
                    await transaction.CommitAsync();
                    return response;
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return new List<Booking>();
        }

        private async Task<Booking> SingleBooking(BookingDTO booking)
        {
            // try and book
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var response = await _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time
                    && x.Id == booking.Id)
                .FirstOrDefaultAsync();

                var member = MapToMember(booking);

                if (response == null)
                {
                    return new Booking();
                }

                response.Member = member;

                _context.UpdateRange(response);
                var numbersUpdated = await _context.SaveChangesAsync();

                if (numbersUpdated == 2)
                {
                    await transaction.CommitAsync();
                    return response;
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return new Booking();
        }

        private Member MapToMember(BookingDTO bookingDTO)
        {
            var member = new Member
            {
                Name = bookingDTO.Member.Name,
                Surname = bookingDTO.Member.Surname,
                Mobile = bookingDTO.Member.Mobile
            };

            return member;
        }

        private List<Member> MapToMembers(BookingDTO bookingDTO)
        {
            var members = new List<Member>();

            foreach (var member in bookingDTO.Members)
            {
                members.Add(
                    new Member
                    {
                        Name = member.Name,
                        Surname = member.Surname,
                        Mobile = member.Mobile
                    }
               );
            }


            return members;
        }

        private List<BookingDTO> MapToBookingDTO(List<Booking> bookings)
        {
            var dto = new List<BookingDTO>();

            foreach (var booking in bookings)
            {
                dto.Add(
                    new BookingDTO
                    {
                        Id = booking.Id,
                        ServiceId = booking.ServiceId,
                        Date = booking.Date,
                        Time = booking.Time
                    }
                );
            }

            return dto;
        }
    }
}
