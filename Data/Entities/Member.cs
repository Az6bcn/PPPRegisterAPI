using System;
namespace CheckinPPP.Data.Entities
{
    public class Member
    {
        public Member()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
