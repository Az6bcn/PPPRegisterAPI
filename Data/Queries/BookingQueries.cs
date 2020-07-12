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

        public async Task<Booking> GetAvailableSingleBookingsAsync(BookingDTO booking, int category)
        {
            Booking fetchedBooking;

            var response = _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time);

            if (category == 1)
            {
                fetchedBooking = await response
                    .Where(x => x.IsAdultSlot)
                    .FirstOrDefaultAsync();

                return fetchedBooking;
            }
            if (category == 2)
            {
                fetchedBooking = await response
                    .Where(x => x.IsKidSlot)
                    .FirstOrDefaultAsync();

                return fetchedBooking;
            }

            fetchedBooking = await response
                .Where(x => x.IsToddlerSlot)
                .FirstOrDefaultAsync();

            return fetchedBooking;
        }

        public async Task<IEnumerable<Booking>> GetAvailableGroupBookingsAsync(BookingDTO booking, List<int> categories)
        {
            var category1Count = categories.Where(x => x == 1).Count();
            var category2Count = categories.Where(x => x == 2).Count();
            var category3Count = categories.Where(x => x == 3).Count();

            var bookings = new List<Booking>();

            var response = _context.Set<Booking>()
                .Where(x => x.MemberId == null
                    && x.Date.Date == booking.Date.Date
                    && x.Time == booking.Time);

            // category 1
            var res = await response
                .Where(x => x.IsAdultSlot)
                .Take(category1Count)
                .ToListAsync();

            // category 2
            var res2 = await response
               .Where(x => x.IsKidSlot)
               .Take(category2Count)
               .ToListAsync();

            // category 3
            var res3 = await response
               .Where(x => x.IsToddlerSlot)
               .Take(category3Count)
               .ToListAsync();

            bookings.AddRange(res);
            bookings.AddRange(res2);
            bookings.AddRange(res3);

            return bookings;
        }

        public async Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname)
        {
            var response = await _context.Set<Booking>()
                .Include(x => x.Member)
                .Where(x => x.Id == bookingId
                    && x.Member.EmailAddress == email
                    && x.Member.Name == name
                    && x.Member.Surname == surname)
                .ToListAsync();

            if (response.Any())
            {
                return true;
            }

            return false;

        }

        public async Task<Booking> FindBookingByIdAsync(int bookingId)
        {
            var response = await _context.Set<Booking>()
                .Include(x => x.Member)
                .Where(x => x.Id == bookingId)
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<IEnumerable<Booking>> FindBookingsByGoupLinkIdAsync(Guid bookingId)
        {
            var response = await _context.Set<Booking>()
                .Include(x => x.Member)
                .Where(x => x.GroupLinkId == bookingId)
                .ToListAsync();

            return response;
        }

        public async Task CancelBookingAsync(Booking booking)
        {
            _context.Update(booking);

            await _context.SaveChangesAsync();
        }

        public async Task CancelBookingsAsync(IEnumerable<Booking> booking)
        {
            _context.UpdateRange(booking);

            await _context.SaveChangesAsync();
        }

        public async Task<Member> FindMemberByEmailAsync(string email, MemberDTO member)
        {
            var response = await _context.Set<Member>()
                .FirstOrDefaultAsync(x => x.EmailAddress == email
                && x.Name == member.Name
                && x.Surname == member.Surname);

            return response;
        }


        public async Task<IEnumerable<Member>> FindMembersOfGroupBookingByEmailAsync(string email)
        {
            var response = await _context.Set<Member>()
                .Where(x => x.EmailAddress == email)
                .ToListAsync();

            return response;
        }

        public async Task<BookingsUpdateSignalR> GetBookingsUpdateAsync(int serviceId, DateTime date, string time)
        {
            var bookings = await _context.Set<Booking>()
                .Where(x => x.ServiceId == serviceId
                    && x.Date.Date == date.Date
                    && x.Time == time
                    && x.MemberId == null)
                .ToListAsync();

            var availableBookings = bookings
                .Where(x => x.MemberId == null)
                .ToList();

            var bookingsUpdate = new BookingsUpdateSignalR
            {
                ServiceId = serviceId,
                Total = bookings.Count(),
                Time = time,
                AdultsAvailableSlots = availableBookings.Where(x => x.IsAdultSlot).Count(),
                KidsAvailableSlots = availableBookings.Where(x => x.IsKidSlot).Count(),
                ToddlersAvailableSlots = availableBookings.Where(x => x.IsToddlerSlot).Count()
            };

            return bookingsUpdate;
        }

    }
}
