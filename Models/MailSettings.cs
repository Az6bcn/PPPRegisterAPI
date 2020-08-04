using System;
namespace CheckinPPP.Models
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ReturnUri { get; set; }
        public string PasswordResetUrl { get; set; }
    }
}
