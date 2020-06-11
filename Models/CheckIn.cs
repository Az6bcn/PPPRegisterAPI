using System;
using System.ComponentModel.DataAnnotations;

namespace CheckinPPP.Models
{
    public class CheckIn
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Mobile { get; set; }
    }
}
