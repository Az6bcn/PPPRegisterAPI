using System;
using System.Collections.Generic;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;

namespace CheckInPPP.Tests.Helpers
{
    public static class BookingsTest
    {
        public static BookingDTO GetSingleBooking()
        {
            var booking = new BookingDTO
            {
                ServiceId = 1,
                Time = "08:30",
                EmailAddress = "a@a.com",
                Date = DateTime.Now.Date.AddDays(3),
                Mobile = "0000000000000",
                Member = new MemberDTO
                {
                    EmailAddress = "a@a.com",
                    CategoryId = 1, // adult
                    Gender = "Male",
                    Mobile = "0000000000000",
                    Name = "azeez",
                    Surname = "odumosu",
                    PickUp = false
                }
            };

            return booking;
        }

        public static BookingDTO GetGroupBooking()
        {
            var booking = new BookingDTO
            {
                ServiceId = 1,
                Time = "08:30",
                EmailAddress = "a@a.com",
                Date = DateTime.Now.Date.AddDays(3),
                Mobile = "0000000000000",
                Members = new List<MemberDTO> {
                    new MemberDTO
                    {
                        EmailAddress = "a@a.com",
                        CategoryId = 1, // adult
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "azeez",
                        Surname = "odumosu",
                        PickUp = false
                    },
                     new MemberDTO
                    {
                        EmailAddress = "a@a.com",
                        CategoryId = 1, // adult
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "larry",
                        Surname = "odumosu",
                        PickUp = false
                    },
                      new MemberDTO
                    {
                        EmailAddress = "a@a.com",
                        CategoryId = 1, // adult
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "sergio",
                        Surname = "odumosu",
                        PickUp = false
                    }
                }
            };

            return booking;
        }

        public static Booking MapToBooking(BookingDTO bookingDTO)
        {
            var booking = new Booking
            {
                Id = 1,
                ServiceId = 1,
                IsAdultSlot = true,
                //BookingReference = Guid.NewGuid(),
                Date = bookingDTO.Date,
                Member = new Member
                {
                    Id = 1,
                    Gender = bookingDTO.Member.Gender,
                    EmailAddress = bookingDTO.Member.EmailAddress,
                    Mobile = bookingDTO.Member.Mobile,
                    Name = bookingDTO.Member.Name,
                    Surname = bookingDTO.Member.Surname,
                    CreatedAt = DateTime.Now
                },
                MemberId = 1,
                Time = "08:30",
                PickUp = bookingDTO.Member.PickUp
            };

            return booking;
        }

        public static IEnumerable<Booking> AvailablBookings()
        {
            var booking = new List<Booking> {
                new Booking
                {
                    Id = 1,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 9,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 10,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 11,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 2,
                    ServiceId = 1,
                    IsKidSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 3,
                    ServiceId = 1,
                    IsToddlerSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 4,
                    ServiceId = 2,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "10:10",
                    PickUp = false
                },
                new Booking
                {
                    Id = 5,
                    ServiceId = 2,
                    IsKidSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "10:10",
                    PickUp = false
                },
                new Booking
                {
                    Id = 5,
                    ServiceId = 2,
                    IsToddlerSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "10:10",
                    PickUp = false
                },
                new Booking
                {
                    Id = 6,
                    ServiceId = 3,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "11:50",
                    PickUp = false
                },
                new Booking
                {
                    Id = 7,
                    ServiceId = 3,
                    IsKidSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "11:50",
                    PickUp = false
                },
                new Booking
                {
                    Id = 8,
                    ServiceId = 3,
                    IsToddlerSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "11:50",
                    PickUp = false
                },
            };

            return booking;
        }

        public static IEnumerable<Booking> ExistingGroupBookings(Guid _groupId)
        {
            var groupId = _groupId;

            var booking = new List<Booking> {
                new Booking
                {
                    Id = 1,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Time = "08:30",
                    PickUp = false,
                    GroupLinkId = groupId,
                    Member = new Member
                    {
                        Id = 1,
                        EmailAddress = "a@a.com",
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "azeez",
                        Surname = "odumosu"
                    }
                },
                new Booking
                {
                    Id = 9,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Time = "08:30",
                    PickUp = false,
                    GroupLinkId = groupId,
                    Member = new Member
                    {
                        Id = 2,
                        EmailAddress = "a@a.com",
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "larry",
                        Surname = "odumosu"
                    }
                },
                new Booking
                {
                    Id = 10,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Time = "08:30",
                    PickUp = false,
                    GroupLinkId = groupId,
                    Member = new Member
                    {
                        Id = 3,
                        EmailAddress = "a@a.com",
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "sergio",
                        Surname = "odumosu"
                    }
                },
                new Booking
                {
                    Id = 11,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 2,
                    ServiceId = 1,
                    IsKidSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 3,
                    ServiceId = 1,
                    IsToddlerSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                }
            };

            return booking;
        }
        public static IEnumerable<Booking> ExistingGroupBookingsLastMemberMissing(Guid _groupId)
        {
            var groupId = _groupId;

            var booking = new List<Booking> {
                new Booking
                {
                    Id = 1,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Time = "08:30",
                    PickUp = false,
                    GroupLinkId = groupId,
                    Member = new Member
                    {
                        Id = 1,
                        EmailAddress = "a@a.com",
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "azeez",
                        Surname = "odumosu"
                    }
                },
                new Booking
                {
                    Id = 9,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Time = "08:30",
                    PickUp = false,
                    GroupLinkId = groupId,
                    Member = new Member
                    {
                        Id = 2,
                        EmailAddress = "a@a.com",
                        Gender = "Male",
                        Mobile = "0000000000000",
                        Name = "larry",
                        Surname = "odumosu"
                    }
                },
                new Booking
                {
                    Id = 11,
                    ServiceId = 1,
                    IsAdultSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 2,
                    ServiceId = 1,
                    IsKidSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                },
                new Booking
                {
                    Id = 3,
                    ServiceId = 1,
                    IsToddlerSlot = true,
                    BookingReference = null,
                    Date = DateTime.Now.AddDays(3),
                    Member = null,
                    Time = "08:30",
                    PickUp = false
                }
            };

            return booking;
        }


    }
}
