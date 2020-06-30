using System;
using System.Globalization;

namespace CheckinPPP.DTOs
{
    public class SignInOutDTO
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int ServiceId { get; set; }
        public string Time { get; set; }
    }
}
