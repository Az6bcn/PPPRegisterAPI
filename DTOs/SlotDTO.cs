using System;
namespace CheckinPPP.DTOs
{
    public class SlotDTO
    {
        public int ServiceId { get; set; }
        public string Time { get; set; }
        public int AdultsAvailableSlots { get; set; }
        public int KidsAvailableSlots { get; set; }
        public int ToddlersAvailableSlots { get; set; }
        public string ServiceName { get; set; }
        public DateTime SpecialServiceDate { get; set; }
        public bool ShowSpecialService { get; set; }
        public bool ShowSpecialServiceSlotDetails { get; set; }
        public string SpecialAnnouncement { get; set; }
        public bool HasSpecialAnnouncement { get; set; }
        public string SpecialServiceYoutubeUrl { get; set; }
    }
}
