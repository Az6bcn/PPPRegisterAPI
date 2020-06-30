using System;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public class SeedSecondService
    {
        public static IEnumerable<Booking> SeedSecondServiceBookingData()
        {
            var max = SeedHelper.Max;
            var data = new List<Booking>();

            var dates = SeedHelper.GetDateTimes();

            foreach (var date in dates)
            {
                for (int i = 0; i <= max - 1; i++)
                {
                    data.Add(
                            new Booking
                            {
                                ServiceId = (int)ServiceTypesEnum.SecondService,
                                Time = "11:50",
                                Date = date
                            }
                        );
                }
            }

            return data;
        }
    }
}
