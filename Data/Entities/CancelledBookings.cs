using System;
namespace CheckinPPP.Data.Entities
{
    public class CancelledBooking
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTime CancelledAt { get; set; }
        public Member Member { get; set; }
        public int MemberId { get; set; }
        public Booking Booking { get; set; }
        public Guid BookingReferenceId { get; set; }
    }
}
