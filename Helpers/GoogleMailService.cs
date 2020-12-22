using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CheckinPPP.Helpers
{
    public class GoogleMailService : IGoogleMailService
    {
        private readonly ILogger<GoogleMailService> _logger;
        private readonly MailSettings _mailSettings;

        public GoogleMailService(IOptions<MailSettings> mailSettings, ILogger<GoogleMailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        // https://www.codewithmukesh.com/blog/send-emails-with-aspnet-core/

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var message = new MailMessage();
            var smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;

            message.IsBodyHtml = false;
            message.Body = mailRequest.Body;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

        public async Task SendBookingConfirmationEmailAsync(string email, Booking booking)
        {
            var date = DateTime.ParseExact(booking.Date.Date.Add(GetTime(booking)).ToString(), "MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            var dateString = date.ToString("dd/MM/yyyy");

            var message = new MailMessage();
            var smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(email));
            message.Subject = $"Booking Confirmation for {dateString}";

            message.IsBodyHtml = false;
            message.IsBodyHtml = true;
            message.Body = HtmlTemplate(booking, email);
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.BookingError, ex,
                    "Could not send email to user: {user} for this booking details: {booking}", email, booking);
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, ApplicationUser user, string token)
        {
            var message = new MailMessage();
            var smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(email));
            message.Subject = "Password reset";

            message.IsBodyHtml = false;
            message.IsBodyHtml = true;
            message.Body = HtmlTemplatePasswordReset(user, token, email);
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.PasswordReset, ex,
                    "Could not send password reset email to user: {user}", user);
            }
        }

        private TimeSpan GetTime(Booking booking)
        {
            var time = booking.Time.Split(':');

            var hours = Convert.ToInt32(time[0]);
            var minutes = Convert.ToInt32(time[1]);

            var timeSpan = new TimeSpan(hours, minutes, 0);

            return timeSpan;
        }

        private string HtmlTemplate(Booking booking, string email)
        {
            var date = DateTime.ParseExact(booking.Date.Date.ToString(), "MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            var dateString = date.ToString("dd/MM/yyyy");

            var template = "<h3>Booking Confirmation</h3>" +
                           $"<p>Dear {booking.User.Name} {booking.User.Surname}, </p> " +
                           $"<p> Your booking reference is {booking.BookingReference}, your reservation to come to church on the <strong>{dateString}</strong> for the time slot <strong>{booking.Time}</strong> has been confirmed.</p>" +
                           "<p> To cancel this booking, please click on the button below. " +
                           "</p>" +
                           $"<a href=\"{new Uri($"{_mailSettings.ReturnUri}?bookingId={booking.Id}&email={email}&name={booking.User.Name}&surname={booking.User.Surname}")}\"> Cancel </a>" +
                           "</br >" +
                           "<p> Precious People Parish </p>" +
                           "<p> 0161 835 9000, 07535 703 955 </p>";

            return template;
        }

        private string HtmlTemplatePasswordReset(ApplicationUser user, string token, string email)
        {
            var template = "<h3>Password Reset</h3>" +
                           $"<p>Dear {user.Name} {user.Surname}, </p> " +
                           "<p> To reset your password, click the link below </p>" +
                           $"<a href=\"{new Uri($"{_mailSettings.PasswordResetUrl}?token={token}&email={email}")}\"> Reset Password </a>" +
                           "</br >" +
                           "<p> If you did not forgot your password you can safely ignore this email.</p>" +
                           "</br >" +
                           "<p> Precious People Parish </p>" +
                           "<p> 0161 835 9000, 07535 703 955 </p>";

            return template;
        }
    }
}