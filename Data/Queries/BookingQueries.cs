using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Data.Queries
{
    public class BookingQueries : IBookingQueries
    {
        private readonly ApplicationDbContext _context;
        public BookingQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAvailableBookingsAsync(DateTime date)
        {
            var response = await _context.Set<Booking>()
                .Where(x => x.Date.Date == date.Date
                    && x.MemberId == null)
                .ToListAsync();

            return response;
        }

        public async Task<IEnumerable<Booking>> GetAvailableBookingsAsync(int serviceId, DateTime date, string time)
        {
            var response = await _context.Set<Booking>()
                .Where(x => x.ServiceId == serviceId
                    && x.Date.Date == date.Date
                    && x.Time == time
                    && x.MemberId == null)
                .ToListAsync();

            return response;
        }

        public async Task<Booking> GetAvailableSingleBookingsAsync(BookingDTO booking)
        {
            var response = await _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time
                    )
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<IEnumerable<Booking>> GetAvailableGroupBookingsAsync(BookingDTO booking)
        {
            var response = await _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time)
                .Take(booking.Members.Count)
                .ToListAsync();

            return response;
        }
    }
}
