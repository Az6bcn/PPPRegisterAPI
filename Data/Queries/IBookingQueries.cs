using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Identity;

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
        Task<ApplicationUser> FindUserByIdAsync(string Id);
        Task<IdentityResult> CreateUserAsnc(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> FindUsersAssignedToMainUserInGroupBokingByEmailAsync(string email);
        Task<IEnumerable<Booking>> FindBookingByUserAndDateAsync(string userId, DateTime date);
    }
}
