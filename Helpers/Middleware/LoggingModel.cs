namespace CheckinPPP.Helpers.Middleware
{
    public class LoggingModel
    {
        public string Method { get; set; }
        public string RequestPath { get; set; }
        public string QueryString { get; set; }
        public string Host { get; set; }
        public string ClientMachineName { get; set; }
        public string ClientIp { get; set; }
        public string Duration { get; set; }
        public string Token { get; set; }
        public string Body { get; set; }
    }
}