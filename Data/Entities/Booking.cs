using System;
using CheckinPPP.Models;

namespace CheckinPPP.Data.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public DateTime? Cancelled { get; set; }
        public Guid? GroupLinkId { get; set; }
        public DateTime? SignIn { get; set; }
        public DateTime? SignOut { get; set; }
        public Guid? BookingReference { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public bool PickUp { get; set; }
        public bool IsAdultSlot { get; set; }
        public bool IsKidSlot { get; set; }
        public bool IsToddlerSlot { get; set; }
        public bool IsSpecialService { get; set; }
        public string SpecialServiceName { get; set; }
        public bool ShowSpecialService { get; set; }
        public bool ShowSpecialServiceSlotDetails { get; set; }
        public string ShowSpecialAnnouncement { get; set; }
        public string SpecialServiceYoutubeUrl { get; set; }
        public bool ShowSundayService { get; set; }
    }
}