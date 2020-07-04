using CheckinPPP.Data.Entities;
using RestSharp;

namespace CheckinPPP.Helpers
{
    public interface ISendEmails
    {
        IRestResponse SendBookingCancellation(string email, Booking booking);
        IRestResponse SendBookingCancellationGoneWrong(string email, string name, string surname);
        IRestResponse SendBookingConfirmationTemplate(string email, Booking booking);
    }
}