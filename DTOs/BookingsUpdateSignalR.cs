using System;
namespace CheckinPPP.DTOs
{
    public class BookingsUpdateSignalR
    {
        public int ServiceId { get; set; }
        public int Total { get; set; }
        public int AvailableBookings { get; set; }
        public string Time { get; set; }
    }
}
