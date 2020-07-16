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
                availableBookingSlot.UserId = user.Id;
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
            var users = new List<ApplicationUser>();

            var categoriesInGroupBooking = booking
                .Members
                .Select(x => x.CategoryId).ToList();

            var response = await _bookingQueries.GetAvailableGroupBookingsAsync(booking, categoriesInGroupBooking);

            if (!response.Any() || response.Count() != booking.Members.Count) { return null; }

            // find users assigned to main user's email
            var assignedMembers = await _bookingQueries.FindUsersAssignedToMainUserInGroupBokingByEmailAsync(booking.EmailAddress);

            if (assignedMembers.Any())
            {
                /// check if all the passed in members are in it
                var res = GetAllMemebersInExistingGroupEmail(assignedMembers, MapToApplicationUsers(booking).ToList());

                //if (res.exists.Any()) { users.AddRange(res.exists); }
                //if (res.notExists.Any()) { users.AddRange(res.notExists); }

                AssignExistingAndNonExistingUsersToBookings(response.ToList(), res.exists.ToList(), booking, res.notExists.ToList());
                return response.ToList();
            }
            else
            {
                users.AddRange(MapToApplicationUsers(booking));
            }

            var bookingWithMembersAssigned = AssignUsersToBookings(response, users, booking);

            return response.ToList();
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


        private IEnumerable<Booking> AssignUsersToBookings(IEnumerable<Booking> response, List<ApplicationUser> users, BookingDTO booking)
        {

            var groupId = Guid.NewGuid();
            var bookingReference = Guid.NewGuid();

            var i = 0;
            foreach (var _booking in response)
            {

                _booking.User = users[i];
                _booking.GroupLinkId = groupId;
                _booking.BookingReference = bookingReference;
                _booking.PickUp = booking.Members
                    .Where(x => x.Name == users[i].Name
                        && x.Surname == users[i].Surname)
                    .First().PickUp;

                i++;
            }

            return response;
        }

        private IEnumerable<Booking> AssignExistingAndNonExistingUsersToBookings(List<Booking> response, List<ApplicationUser> existingUsers, BookingDTO booking, List<ApplicationUser> nonExistingUsers)
        {

            var added = new List<int>();
            var groupId = Guid.NewGuid();
            var bookingReference = Guid.NewGuid();

            for (int i = 1; i <= existingUsers.Count(); i++)
            {
                response[i - 1].UserId = existingUsers[i - 1].Id;
                response[i - 1].GroupLinkId = groupId;
                response[i - 1].BookingReference = bookingReference;
                response[i - 1].PickUp = booking.Members
                    .Where(x => x.Name == existingUsers[i - 1].Name
                        && x.Surname == existingUsers[i - 1].Surname)
                    .First().PickUp;

                added.Add(response[i - 1].Id);
            }

            var leftBookings = response
                .Where(x => !added.Contains(x.Id))
                .ToList();


            var j = 0;
            foreach (var _booking in leftBookings)
            {
                _booking.User = nonExistingUsers[j];
                _booking.GroupLinkId = groupId;
                _booking.BookingReference = bookingReference;
                _booking.PickUp = booking.Members
                    .Where(x => x.Name == nonExistingUsers[j].Name
                        && x.Surname == nonExistingUsers[j].Surname)
                    .First().PickUp;

                j++;
            }

            return response;
        }

        private (IEnumerable<ApplicationUser> exists, IEnumerable<ApplicationUser> notExists) GetAllMemebersInExistingGroupEmail(IEnumerable<ApplicationUser> existingMembers, List<ApplicationUser> requestMembers)
        {
            var comparer = new MemberEqualityComparer();
            var notComparer = new DistinctEqualityComparer();

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

        private IEnumerable<ApplicationUser> MapToApplicationUsers(BookingDTO bookingDTO)
        {
            var users = new List<ApplicationUser>();

            foreach (var member in bookingDTO.Members)
            {
                users.Add(
                    new ApplicationUser
                    {
                        Name = member.Name,
                        Surname = member.Surname,
                        Gender = member.Gender,
                        PhoneNumber = bookingDTO.Mobile,
                        Email = bookingDTO.EmailAddress,
                        UserName = bookingDTO.EmailAddress,
                        CreatedAt = DateTime.Now
                    }
               );
            }

            return users;
        }
    }
}
