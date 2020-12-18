using System;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;

namespace CheckinPPP.Helpers
{
    public interface ISeedServices
    {
        IEnumerable<Booking> FirstServiceBookingData();
        IEnumerable<Booking> SeedSecondServiceBookingData();
        IEnumerable<Booking> SeedWorkersServiceBookingData();
    }
}
