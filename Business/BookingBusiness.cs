using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using CheckinPPP.Helpers;
using CheckinPPP.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Business
{
    public class BookingBusiness : IBookingBusiness
    {

        private readonly IBookingQueries _bookingQueries;

        public BookingBusiness(IBookingQueries bookingQueries)
        {
            _bookingQueries = bookingQueries;
        }

        public async Task<Booking> SingleBookingAsync(BookingDTO booking)
        {
            var availableBookingSlot = await _bookingQueries.GetAvailableSingleBookingsAsync(booking, booking.Member.CategoryId);

            if (availableBookingSlot is null)
            {
                return null;
            }

            availableBookingSlot.BookingReference = Guid.NewGuid();
            availableBookingSlot.PickUp = booking.Member.PickUp;


            if (!string.IsNullOrWhiteSpace(booking.Member.Id))
            {
                // find user
                var user = await _bookingQueries.FindUserByIdAsync(booking.Member.Id);
                if (user is null) { return null; }
                availableBookingSlot.UserId = Guid.Parse(user.Id);
            }
            else
            {
                // create user
                var userToCreate = MapToApplicationUser(booking);
                availableBookingSlot.User = userToCreate;
            }

            return availableBookingSlot;
        }

        public async Task<List<Booking>> GroupBookingAsync(BookingDTO booking)
        {
            var members = new List<Member>();

            var categoriesInGroupBooking = booking
                .Members
                .Select(x => x.CategoryId).ToList();

            var response = await _bookingQueries.GetAvailableGroupBookingsAsync(booking, categoriesInGroupBooking);

            if (!response.Any() || response.Count() != booking.Members.Count) { return new List<Booking>(); }

            var existsMembers = await _bookingQueries.FindMembersOfGroupBookingByEmailAsync(booking.EmailAddress);

            if (existsMembers.Any())
            {
                /// check if all the passed in members are in it
                var res = GetAllMemebersInExistingGroupEmail(existsMembers, MapToMembers(booking));

                if (res.exists.Any()) { members.AddRange(res.exists); }
                if (res.notExists.Any()) { members.AddRange(MapToMembers(booking)); }
            }
            else
            {
                members.AddRange(MapToMembers(booking));
            }

            var bookingWithMembersAssigned = AssignMembersToBookings(response, members, booking);

            return response.ToList();
        }

        private Member MapToMember(BookingDTO bookingDTO)
        {
            var member = new Member
            {
                Name = bookingDTO.Member.Name,
                Surname = bookingDTO.Member.Surname,
                Gender = bookingDTO.Member.Gender,
                Mobile = bookingDTO.Mobile,
                EmailAddress = bookingDTO.EmailAddress
            };

            return member;
        }

        public IEnumerable<BookingDTO> MapToBookingDTOs(IEnumerable<Booking> bookings)
        {
            var dto = new List<BookingDTO>();

            foreach (var booking in bookings)
            {
                dto.Add(
                    new BookingDTO
                    {
                        Id = booking.Id,
                        ServiceId = booking.ServiceId,
                        Date = booking.Date,
                        Time = booking.Time
                    }
                );
            }

            return dto;
        }

        public BookingDTO MapToBookingDTO(Booking booking)
        {

            var dto = new BookingDTO
            {
                Id = booking.Id,
                ServiceId = booking.ServiceId,
                Date = booking.Date,
                Time = booking.Time,
            };

            return dto;
        }

        public async Task<bool> IsValidBookingAsync(int bookingId, string email, string name, string surname)
        {
            var isValidBooking = await _bookingQueries.IsValidBookingAsync(bookingId, email, name, surname);

            return isValidBooking;
        }



        private List<Member> MapToMembers(BookingDTO bookingDTO)
        {
            var members = new List<Member>();

            foreach (var member in bookingDTO.Members)
            {
                members.Add(
                    new Member
                    {
                        Name = member.Name,
                        Surname = member.Surname,
                        Gender = member.Gender,
                        Mobile = bookingDTO.Mobile,
                        EmailAddress = bookingDTO.EmailAddress
                    }
               );
            }


            return members;
        }

        private IEnumerable<Booking> AssignMembersToBookings(IEnumerable<Booking> response, List<Member> members, BookingDTO booking)
        {

            var groupId = Guid.NewGuid();
            var bookingReference = Guid.NewGuid();

            var i = 0;
            foreach (var _booking in response)
            {

                _booking.Member = members[i];
                _booking.GroupLinkId = groupId;
                _booking.BookingReference = bookingReference;
                _booking.PickUp = booking.Members
                    .Where(x => x.Name == members[i].Name
                        && x.Surname == members[i].Surname)
                    .First().PickUp;

                i++;
            }

            return response;
        }

        private (IEnumerable<Member> exists, IEnumerable<Member> notExists) GetAllMemebersInExistingGroupEmail(IEnumerable<Member> existingMembers, List<Member> requestMembers)
        {
            var comparer = new MemberEqualityComparer();
            var notComparer = new NotMemberEqualityComparer();

            // are all the members in db same as the ones in the request
            var sameMembers = existingMembers.
                Where(x => requestMembers.Contains(x, comparer))
                .ToList();

            // no existing: the ones in requestMember and not in existingmember
            //var notMembers = requestMembers.Except(sameMembers, notComparer);

            var notMembers = requestMembers
                .Where(x => !sameMembers.Contains(x, comparer))
                .ToList();


            return (sameMembers, notMembers);
        }

        private ApplicationUser MapToApplicationUser(BookingDTO bookingDTO)
        {
            var member = new ApplicationUser
            {
                Name = bookingDTO.Member.Name,
                Surname = bookingDTO.Member.Surname,
                Gender = bookingDTO.Member.Gender,
                PhoneNumber = bookingDTO.Mobile,
                Email = bookingDTO.EmailAddress,
                UserName = bookingDTO.EmailAddress,
                CreatedAt = DateTime.Now
            };

            return member;
        }
    }
}
