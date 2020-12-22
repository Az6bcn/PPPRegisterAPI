using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public class Seed2021Services : ISeedServices
    {
        private readonly ICalculateSundaysIn2021 _calculateSundaysIn2021;

        public Seed2021Services(ICalculateSundaysIn2021 calculateSundaysIn2021)
        {
            _calculateSundaysIn2021 = calculateSundaysIn2021;
        }

        public IEnumerable<Booking> FirstServiceBookingData()
        {
            var max = 100;
            var data = new List<Booking>();

            var dates = _calculateSundaysIn2021.GetSundays2021();

            foreach (var date in dates)
                for (var i = 0; i <= max - 1; i++)
                {
                    if (i <= 69)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.FirstService,
                                Time = "10:10",
                                Date = date,
                                IsAdultSlot = true,
                                IsKidSlot = false,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 70 && i <= 89)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.FirstService,
                                Time = "10:10",
                                Date = date,
                                IsAdultSlot = false,
                                IsKidSlot = true,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 90 && i <= 99)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.FirstService,
                                Time = "10:10",
                                Date = date,
                                IsAdultSlot = false,
                                IsKidSlot = false,
                                IsToddlerSlot = true
                            }
                        );
                }

            return data;
        }


        public IEnumerable<Booking> SeedSecondServiceBookingData()
        {
            var max = 100;
            var data = new List<Booking>();

            var dates = _calculateSundaysIn2021.GetSundays2021();

            foreach (var date in dates)
                for (var i = 0; i <= max - 1; i++)
                {
                    if (i <= 69)
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
                    if (i >= 70 && i <= 89)
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
                    if (i >= 90 && i <= 99)
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


        public IEnumerable<Booking> SeedWorkersServiceBookingData()
        {
            var max = 100;
            var data = new List<Booking>();

            var dates = _calculateSundaysIn2021.GetSundays2021();

            foreach (var date in dates)
                for (var i = 0; i <= max - 1; i++)
                {
                    if (i <= 69)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.WorkersMeeting,
                                Time = "08:30",
                                Date = date,
                                IsAdultSlot = true,
                                IsKidSlot = false,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 70 && i <= 89)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.WorkersMeeting,
                                Time = "08:30",
                                Date = date,
                                IsAdultSlot = false,
                                IsKidSlot = true,
                                IsToddlerSlot = false
                            }
                        );
                    if (i >= 90 && i <= 99)
                        data.Add(
                            new Booking
                            {
                                ServiceId = (int) ServiceTypesEnum.WorkersMeeting,
                                Time = "08:30",
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