using System;
using CheckinPPP.Models;

namespace CheckinPPP.Data.Entities
{
    public class CancelledBooking
    {
        public int Id { get; set; }
        public DateTime CancelledAt { get; set; }
        public Booking Booking { get; set; }
        public int BookingId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}