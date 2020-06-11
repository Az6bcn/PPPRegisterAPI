using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.DTOs
{
    public class CheckedInMembersDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public DateTime CheckedInAt { get; set; }
    }
}
