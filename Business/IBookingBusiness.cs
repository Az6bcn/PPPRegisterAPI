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
        IEnumerable<BookingDTO> MapToBookingDTOs(IEnumerable<Booking> bookings);
        Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname);
        BookingDTO MapToBookingDTO(Booking booking);
    }
}
