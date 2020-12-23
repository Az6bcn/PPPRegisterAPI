namespace CheckinPPP.Models
{
    public class MailGunApiEmailSettings
    {
        public string ApiKey { get; set; }

        public string BaseUri { get; set; }

        public string RequestUri { get; set; }

        public string From { get; set; }

        public string ReturnUri { get; set; }
    }
}