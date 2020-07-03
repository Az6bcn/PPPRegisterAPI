using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;

namespace CheckinPPP.Business
{
    public interface IBookingBusiness
    {
        Task<Booking> SingleBookingAsync(BookingDTO booking);
        Task<List<Booking>> GroupBookingAsync(BookingDTO booking);
        Task<BookingsUpdateSignalR> GetBookingsUpdateAsync(int serviceId, DateTime date, string time);
        IEnumerable<BookingDTO> MapToBookingDTO(IEnumerable<Booking> bookings);
    }
}
