using System;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace CheckinPPP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Bookings = new List<Booking>();
            CancelledBookings = new List<CancelledBooking>();
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<CancelledBooking> CancelledBookings { get; set; }
    }
}
