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
        Task<Booking> GetAvailableSingleBookingsAsync(BookingDTO booking);
        Task<IEnumerable<Booking>> GetAvailableGroupBookingsAsync(BookingDTO booking);
    }
}
