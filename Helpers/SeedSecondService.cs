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
                for (var i = 0; i <= max - 1; i++)
                {
                    if (i <= 49)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.SecondService,
                                Time = "11:50",
                                Date = date,
                                IsAdultSlot = true,
                                IsKidSlot = false,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 50 && i <= 69)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.SecondService,
                                Time = "11:50",
                                Date = date,
                                IsAdultSlot = false,
                                IsKidSlot = true,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 70 && i <= 79)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.SecondService,
                                Time = "11:50",
                                Date = date,
                                IsAdultSlot = false,
                                IsKidSlot = false,
                                IsToddlerSlot = true
                            }
                        );
                }

            return data;
        }
    }
}