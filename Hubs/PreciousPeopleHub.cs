using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.Hubs
{
    // methods that will be called by the client.
    public class PreciousPeopleHub : Hub<IPreciousPeopleClient>
    {
        private readonly ApplicationDbContext _context;
        public PreciousPeopleHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateCheckedInMemebers(CheckedInMemberDTO checkedInMembers)
        {
            await Clients.Others.UpdateCheckedInMembersAsync(checkedInMembers);
        }

        // client to call this method to get booking update
        public async Task GetBookingsUpdate(int serviceId, DateTime date, string time)
        {
            var bookings = await _context.Set<Booking>()
                .Where(x => x.ServiceId == serviceId
                    && x.Date.Date == date.Date
                    && x.Time == time)
                .ToListAsync();

            var availableBookings = bookings
                .Where(x => x.MemberId == null)
                .ToList();

            var bookingsUpdate = new BookingsUpdateSignalR
            {
                Total = bookings.Count(),
                ServiceId = serviceId,
                Time = time,
                AdultsAvailableSlots = bookings.Where(x => x.IsAdultSlot).Count(),
                KidsAvailableSlots = bookings.Where(x => x.IsKidSlot).Count(),
                ToddlersAvailableSlots = bookings.Where(x => x.IsToddlerSlot).Count()
            };

            // send available book to all clients: client need to implement ReceivedBookingsUpdateAsync to receive updates
            await Clients.All.ReceivedBookingsUpdateAsync(bookingsUpdate);
        }

        public async Task UpdateSlotsAvailable(DateTime date)
        {

            await Clients.All.UpdateSlotsAvailableAsync(date);
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
