using System;
using System.Collections;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using CheckinPPP.Migrations;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public class SeedFirstService
    {
        public static IEnumerable<Booking> FirstServiceBookingData()
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
                                ServiceId = (int)ServiceTypesEnum.FirstService,
                                Time = "10:10",
                                Date = date
                            }
                        );
                }
            }

            return data;
        }
    }
}
