using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckinPPP.Helpers
{
    public class SeedHelper
    {
        public static int Max => 65;

        public static List<DateTime> GetDateTimes()
        {
            var dates = new List<DateTime>() {
                new DateTime(2020,07,05),
                new DateTime(2020,07,12),
                new DateTime(2020,07,19),
                new DateTime(2020,07,26),
                new DateTime(2020,08,02),
                new DateTime(2020,08,09),
                new DateTime(2020,08,16),
                new DateTime(2020,08,23),
                new DateTime(2020,08,30),
                new DateTime(2020,09,06),
                new DateTime(2020,09,13),
                new DateTime(2020,09,20),
                new DateTime(2020,09,27),
                new DateTime(2020,10,04),
                new DateTime(2020,10,11),
                new DateTime(2020,10,18),
                new DateTime(2020,10,25),
                new DateTime(2020,11,01),
                new DateTime(2020,11,08),
                new DateTime(2020,11,15),
                new DateTime(2020,11,22),
                new DateTime(2020,11,29),
                new DateTime(2020,12,06),
                new DateTime(2020,12,13),
                new DateTime(2020,12,20),
                new DateTime(2020,12,27)
            };

            return dates;
        }
    }
}
