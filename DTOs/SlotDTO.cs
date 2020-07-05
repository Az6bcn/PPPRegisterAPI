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
    }
}
