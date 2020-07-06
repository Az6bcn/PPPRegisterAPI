using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Business
{
    public class BookingBusiness : IBookingBusiness
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingQueries _bookingQueries;

        public BookingBusiness(
            ApplicationDbContext context,
            IBookingQueries bookingQueries
            )
        {
            _context = context;
            _bookingQueries = bookingQueries;
        }

        public async Task<Booking> SingleBookingAsync(BookingDTO booking)
        {
            // try and book
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var response = await _bookingQueries.GetAvailableSingleBookingsAsync(booking, booking.Member.CategoryId);

                if (response is null)
                {
                    return null;
                }

                var bookingReference = Guid.NewGuid();
                response.BookingReference = bookingReference;
                response.PickUp = booking.Member.PickUp;

                var member = MapToMember(booking);
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

        public async Task<List<Booking>> GroupBookingAsync(BookingDTO booking)
        {
            // try and book
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var categoriesInGroupBooking = booking
                    .Members
                    .Select(x => x.CategoryId).ToList();

                var response = await _bookingQueries.GetAvailableGroupBookingsAsync(booking, categoriesInGroupBooking);

                if (!response.Any() || response.Count() != booking.Members.Count)
                {
                    return new List<Booking>();
                }

                var members = MapToMembers(booking);
                var groupId = Guid.NewGuid();
                var bookingReference = Guid.NewGuid();

                var i = 0;
                foreach (var _booking in response)
                {

                    _booking.Member = members[i];
                    _booking.GroupLinkId = groupId;
                    _booking.BookingReference = bookingReference;
                    _booking.PickUp = booking.Members
                        .Where(x => x.Name == members[i].Name
                            && x.Surname == members[i].Surname)
                        .First().PickUp;

                    i++;
                }

                _context.UpdateRange(response);
                var numbersUpdated = await _context.SaveChangesAsync();

                if (numbersUpdated == (booking.Members.Count * 2))
                {
                    await transaction.CommitAsync();
                    return response.ToList();
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

        private Member MapToMember(BookingDTO bookingDTO)
        {
            var member = new Member
            {
                Name = bookingDTO.Member.Name,
                Surname = bookingDTO.Member.Surname,
                Gender = bookingDTO.Member.Gender,
                Mobile = bookingDTO.Mobile,
                EmailAddress = bookingDTO.EmailAddress
            };

            return member;
        }

        public IEnumerable<BookingDTO> MapToBookingDTOs(IEnumerable<Booking> bookings)
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

        public BookingDTO MapToBookingDTO(Booking booking)
        {

            var dto = new BookingDTO
            {
                Id = booking.Id,
                ServiceId = booking.ServiceId,
                Date = booking.Date,
                Time = booking.Time,
            };

            return dto;
        }

        public async Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname)
        {
            var isValidBooking = await _bookingQueries.IsValidBookingAsync(bookingId, email, name, surname);

            return isValidBooking;
        }

        public async Task CancelBookingInsertionAsync(CancelledBooking cancelledBooking)
        {
            _context.Add<CancelledBooking>(cancelledBooking);

            await _context.SaveChangesAsync();
        }

        public async Task CancelBookingsInsertionAsync(IEnumerable<CancelledBooking> cancelledBookings)
        {
            await _context.AddRangeAsync(cancelledBookings);

            await _context.SaveChangesAsync();
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
                        Gender = member.Gender,
                        Mobile = bookingDTO.Mobile,
                        EmailAddress = bookingDTO.EmailAddress
                    }
               );
            }


            return members;
        }
    }
}
