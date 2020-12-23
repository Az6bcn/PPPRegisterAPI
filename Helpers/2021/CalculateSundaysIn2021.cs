using System;
using System.Collections.Generic;

namespace CheckinPPP.Helpers
{
    public class CalculateSundaysIn2021 : ICalculateSundaysIn2021
    {
        public List<DateTime> GetSundays2021()
        {
            var sundays = new List<DateTime>();
            var date = new DateTime(2021, 01, 01);


            for (var i = 1; i <= 365; i++)
            {
                if (date.DayOfWeek == DayOfWeek.Sunday) sundays.Add(date);

                date = date.AddDays(1);
            }

            return sundays;
        }
    }
}