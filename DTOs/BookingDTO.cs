using System;
using System.Collections.Generic;

namespace CheckinPPP.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public DateTime? Cancelled { get; set; }
        public bool IsGroupBooking { get; set; }
        public int TotalNumberBookings { get; set; }
        public Guid? GroupLinkId { get; set; }
        public MemberDTO Member { get; set; }
        public List<MemberDTO> Members { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
    }
}
