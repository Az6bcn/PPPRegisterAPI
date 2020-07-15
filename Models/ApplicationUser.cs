using System;
using Microsoft.AspNetCore.Identity;

namespace CheckinPPP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
