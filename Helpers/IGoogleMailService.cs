using System;
using System.Threading.Tasks;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public interface IGoogleMailService
    {
        Task SendBookingConfirmationEmailAsync(string email, Booking booking);
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendPasswordResetEmailAsync(string email, ApplicationUser user, string token);
    }
}
