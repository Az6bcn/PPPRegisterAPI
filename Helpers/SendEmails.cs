using System;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace CheckinPPP.Helpers
{
    public class SendEmails : ISendEmails
    {
        private readonly MailGunApiEmailSettings _emailOptions;

        public SendEmails(IOptions<MailGunApiEmailSettings> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public IRestResponse SendBookingConfirmationTemplate(string email, Booking booking)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri($"{_emailOptions.BaseUri}");
            client.Authenticator =
            new HttpBasicAuthenticator("api", $"{_emailOptions.ApiKey}");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", $"{_emailOptions.RequestUri}", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{_emailOptions.From}");
            request.AddParameter("to", $"{booking.User.Name} {booking.User.Surname} <{email}>");
            request.AddParameter("subject", $"Booking Confirmation for {booking.Date.Date.Add(GetTime(booking))}");
            request.AddParameter("html", $"<html>{EncodeHtmlTemplate(booking, email)}</html>");
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public IRestResponse SendBookingCancellation(string email, Booking booking)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri($"{_emailOptions.BaseUri}");
            client.Authenticator =
            new HttpBasicAuthenticator("api", $"{_emailOptions.ApiKey}");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", $"{_emailOptions.RequestUri}", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{_emailOptions.From}");
            request.AddParameter("to", $"{booking.User.Name} {booking.User.Surname} <{email}>");
            request.AddParameter("subject", $"Booking Cancellaction for {booking.Date.Date.Add(GetTime(booking))}");
            request.AddParameter("text", $" Dear {booking.User.Name} {booking.User.Surname}, " +
                $"Your booking to come to church on the {booking.Date.Date} for the time slot {booking.Time} has been cancelled.");
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public IRestResponse SendBookingCancellationGoneWrong(string email, string name, string surname)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri($"{_emailOptions.BaseUri}");
            client.Authenticator =
            new HttpBasicAuthenticator("api", $"{_emailOptions.ApiKey}");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", $"{_emailOptions.RequestUri}", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{_emailOptions.From}");
            request.AddParameter("to", $"{name} {surname} <{email}>");
            request.AddParameter("subject", $"Booking Cancellaction");
            request.AddParameter("text", $"Dear {name} {surname}, " +
                $"Your booking could not be cancelled at the moment, Please try again later.");
            request.Method = Method.POST;
            return client.Execute(request);
        }

        private string EncodeHtmlTemplate(Booking booking, string email)
        {
            var template = $"<h3>Booking Confirmation</h3>" +
                $"<p>Dear {booking.User.Name} {booking.User.Surname}, </p> " +
                $"<p> Your booking reference is {booking.BookingReference}, your reservation to come to church on the <strong>{booking.Date.Date}</strong> for the time slot <strong>{booking.Time}</strong> has been confirmed.</p>" +
                $"<p> To cancel this booking, please click on the button below. " +
                $"</p>" +
                $"<button type=\"submit\"><a href=\"{ new Uri($"{_emailOptions.ReturnUri}?bookingId={booking.Id}&email={email}&name={booking.User.Name}&surname={booking.User.Surname}")}\"> Cancel </a> </button>" +
                $"</br >" +
                $"<p> Precious People Parish </p>" +
                $"<p> 0161 835 9000, 07535 703 955 </p>";

            return template;
        }


        private TimeSpan GetTime(Booking booking)
        {
            var time = booking.Time.Split(':');

            var hours = Convert.ToInt32(time[0]);
            var minutes = Convert.ToInt32(time[1]);

            var timeSpan = new TimeSpan(hours, minutes, 0);

            return timeSpan;
        }

    }
}
