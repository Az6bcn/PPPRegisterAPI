using System;

namespace CheckinPPP.DTOs
{
    public class CheckedInMemberDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public DateTime? SignedIn { get; set; }
        public DateTime? SignedOut { get; set; }
        public int ServiceId { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }
        public bool PickUp { get; set; }
        public string Gender { get; set; }
    }
}