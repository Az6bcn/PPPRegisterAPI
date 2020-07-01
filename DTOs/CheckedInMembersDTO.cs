using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.DTOs
{
    public class CheckedInMemberDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public DateTime? SignedIn { get; set; }
        public DateTime? SignedOut { get; set; }
        public int ServiceId { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }
    }
}
