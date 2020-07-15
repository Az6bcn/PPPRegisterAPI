using System;
namespace CheckinPPP.Models
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Authority { get; set; }
    }
}
