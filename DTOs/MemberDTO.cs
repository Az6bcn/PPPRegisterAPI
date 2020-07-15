using System;
namespace CheckinPPP.DTOs
{
    public class MemberDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }

        public bool PickUp { get; set; }

        // 1: adult, 2: kid 3: toddler
        public int CategoryId { get; set; }
    }
}
