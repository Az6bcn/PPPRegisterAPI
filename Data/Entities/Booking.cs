using System;
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
        public Member Member { get; set; }
        public int? MemberId { get; set; }
        public DateTime? SignIn { get; set; }
        public DateTime? SignOut { get; set; }
    }
}
