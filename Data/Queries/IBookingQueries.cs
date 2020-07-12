using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;

namespace CheckinPPP.Data.Queries
{
    public interface IBookingQueries
    {
        Task<IEnumerable<Booking>> GetAvailableBookingsAsync(DateTime date);
        Task<IEnumerable<Booking>> GetAvailableBookingsAsync(int serviceId, DateTime date, string time);
        Task<Booking> GetAvailableSingleBookingsAsync(BookingDTO booking, int category);
        Task<IEnumerable<Booking>> GetAvailableGroupBookingsAsync(BookingDTO booking, List<int> categories);
        Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname);
        Task<Booking> FindBookingByIdAsync(int bookingId);
        Task CancelBookingAsync(Booking booking);
        Task<Member> FindMemberByEmailAsync(string email, MemberDTO member);
        Task<IEnumerable<Booking>> FindBookingsByGoupLinkIdAsync(Guid bookingId);
        Task CancelBookingsAsync(IEnumerable<Booking> booking);
        Task<IEnumerable<Member>> FindMembersOfGroupBookingByEmailAsync(string email);
        Task<BookingsUpdateSignalR> GetBookingsUpdateAsync(int serviceId, DateTime date, string time);
    }
}
