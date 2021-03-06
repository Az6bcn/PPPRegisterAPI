﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;

namespace CheckinPPP.Business
{
    public interface IBookingBusiness
    {
        Task<(Booking booking, bool canBook)> SingleBookingAsync(BookingDTO booking);
        Task<(List<Booking> booking, bool canBook)> GroupBookingAsync(BookingDTO booking);
        IEnumerable<BookingDTO> MapToBookingDTOs(IEnumerable<Booking> bookings);
        Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname);
        BookingDTO MapToBookingDTO(Booking booking, int? totalBooking);
    }
}