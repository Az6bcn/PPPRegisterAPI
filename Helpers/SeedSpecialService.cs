﻿using System;
using System.Collections;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public class SeedSpecialService
    {
        public static Booking SeedSingleSpecialService()
        {
            //var max = 80;

            //var date = new DateTime(2020, 11, 05);

            //var booking = new Booking
            //{
            //    ServiceId = (int)ServiceTypesEnum.SpecialService,
            //    Time = "22:00",
            //    Date = date,
            //    IsAdultSlot = true,
            //    IsKidSlot = false,
            //    IsToddlerSlot = false,
            //    IsSpecialService = true,
            //    SpecialServiceName = "Watch Night 2020"
            //};

            //return booking;
            return null;
        }

        public static IEnumerable<Booking> SeedSpecialServices()
        {

            var max = 240;

            var date = new DateTime(2020, 12, 31);
            var data = new List<Booking>();

            for (int i = 0; i < max; i++)
            {
                data.Add(new Booking
                {
                    ServiceId = (int)ServiceTypesEnum.SpecialService,
                    Time = "23:00",
                    Date = date,
                    IsAdultSlot = true,
                    IsKidSlot = false,
                    IsToddlerSlot = false,
                    IsSpecialService = true,
                    SpecialServiceName = "Crossover Night 2020 (11 PM - 12:00 Midnight)"
                });
            }

            return data;
        }
    }
}
