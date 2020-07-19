﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
using CheckinPPP.Models;
using CheckInPPP.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace CheckInPPP.Tests.Business
{
    [TestClass]
    public class BookingBusinessTests
    {
        private Mock<IBookingQueries> _bookingQueriesMoq;
        private BookingDTO _validSingleBooking;
        private BookingDTO _validGroupBooking;
        private IEnumerable<Booking> _existingGroupBooking;
        private IEnumerable<Booking> _existingGroupBookingUserMissing;
        private Guid groupId = Guid.NewGuid();

        [TestInitialize]
        public void Initialise()
        {
            _bookingQueriesMoq = new Mock<IBookingQueries>(MockBehavior.Strict);

            // single booking
            _validSingleBooking = BookingsTest.GetSingleBooking();

            // group bookings
            _validGroupBooking = BookingsTest.GetGroupBooking();
            _existingGroupBooking = BookingsTest.ExistingGroupBookings(groupId);
            _existingGroupBookingUserMissing = BookingsTest.ExistingGroupBookingsLastUserMissing(groupId);
        }

        #region Single Booking

        [TestMethod]
        public async Task SingleBookingWithExistingUserInDb_WhenCalledWithValidBooking_ShouldReturnObjectWithIdAndUserId()
        {
            // Arrange
            _bookingQueriesMoq
                .Setup(x => x.GetAvailableSingleBookingsAsync(_validSingleBooking, _validSingleBooking.Member.CategoryId))
                .ReturnsAsync((BookingDTO booking, int categoryId) =>
                {
                    var availableBookings = BookingsTest.AvailablBookings()
                        .Where(x => x.ServiceId == booking.ServiceId
                            && x.IsAdultSlot) //because categortId = 1 (adult)
                        .FirstOrDefault();
                    return availableBookings;
                });

            _bookingQueriesMoq
                .Setup(x => x.FindUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) =>
                {
                    return BookingsTest.MapToApplicationuser(_validSingleBooking);
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.SingleBookingAsync(_validSingleBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 1);
            Assert.AreNotEqual(Guid.Empty, result.BookingReference);
            Assert.IsNotNull(result.User);
        }


        [TestMethod]
        public async Task SingleBookingWithNoneExistingUserIndB_WhenCalledWithValidBooking_ShouldReturnObjectWithIdAndUser()
        {
            // Arrange
            _bookingQueriesMoq
                .Setup(x => x.GetAvailableSingleBookingsAsync(_validSingleBooking, _validSingleBooking.Member.CategoryId))
                .ReturnsAsync((BookingDTO booking, int categoryId) =>
                {
                    var availableBookings = BookingsTest.AvailablBookings()
                        .Where(x => x.ServiceId == booking.ServiceId
                            && x.IsAdultSlot) //because categortId = 1 (adult)
                        .FirstOrDefault();
                    return availableBookings;
                });

            _bookingQueriesMoq
                .Setup(x => x.FindUserByIdAsync(It.IsAny<string>()))
                 .ReturnsAsync((string id) =>
                 {
                     return null;
                 });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.SingleBookingAsync(_validSingleBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.BookingReference);
        }


        [TestMethod]
        public async Task SingleBooking_WhenCalledWithNonExistingBooking_ShouldReturnNull()
        {
            // Arrange
            _bookingQueriesMoq
                .Setup(x => x.GetAvailableSingleBookingsAsync(_validSingleBooking, _validSingleBooking.Member.CategoryId))
                .ReturnsAsync((BookingDTO booking, int categoryId) =>
                {
                    return null;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.SingleBookingAsync(_validSingleBooking);

            // Asset
            Assert.IsNull(result);
        }

        #endregion

        #region Group Booking
        [TestMethod]
        public async Task GroupBookingWithExistingUserInDb_WhenCalled_ShouldReturnObjectWithIdAndUsers()
        {
            // Arrange
            var categoriesId = _validGroupBooking.Members.Select(x => x.CategoryId).ToList();

            _bookingQueriesMoq
                .Setup(x => x.GetAvailableGroupBookingsAsync(_validGroupBooking, categoriesId))
                .ReturnsAsync((BookingDTO booking, List<int> categoryId) =>
                {
                    var availableBookings = BookingsTest.AvailablBookings()
                        .Where(x => x.ServiceId == booking.ServiceId
                            && x.IsAdultSlot) //because they are all categortId = 1 (adult)
                        .Take(categoriesId.Count());
                    return availableBookings;
                });

            _bookingQueriesMoq
                .Setup(x => x.FindUsersAssignedToMainUserInGroupBokingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var res = _existingGroupBooking
                    .Where(x => x.ServiceId == 1
                        && x.IsAdultSlot && x.GroupLinkId == groupId)
                    .Select(x => x.User)
                    .ToList();

                    //var response = BookingsTest.MapToApplicationusers(res);

                    return res;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.GroupBookingAsync(_validGroupBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _validGroupBooking.Members.Count());
        }

        [TestMethod]
        public async Task GroupBookingWithNoneExistingUserInDb_WhenCalled_ShouldReturnObjectWithIdAndAllUsers()
        {
            // Arrange
            var categoriesId = _validGroupBooking.Members.Select(x => x.CategoryId).ToList();

            _bookingQueriesMoq
                .Setup(x => x.GetAvailableGroupBookingsAsync(_validGroupBooking, categoriesId))
                .ReturnsAsync((BookingDTO booking, List<int> categoryId) =>
                {
                    var availableBookings = BookingsTest.AvailablBookings()
                        .Where(x => x.ServiceId == booking.ServiceId
                            && x.IsAdultSlot) //because they are all categortId = 1 (adult)
                        .Take(categoriesId.Count());
                    return availableBookings;
                });

            _bookingQueriesMoq
                .Setup(x => x.FindUsersAssignedToMainUserInGroupBokingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    return new List<ApplicationUser>();
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.GroupBookingAsync(_validGroupBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Select(x => x.User).Count(), 3);
            Assert.AreNotEqual(result.Select(x => x.BookingReference), Guid.Empty);
            Assert.AreNotEqual(result.Select(x => x.BookingReference), null);
        }


        [TestMethod]
        public async Task GroupBookingWithSomeExistingUserInDb_WhenCalled_ShouldReturnObjectWithIdAndAllUsers()
        {
            // Arrange
            var categoriesId = _validGroupBooking.Members.Select(x => x.CategoryId).ToList();

            _bookingQueriesMoq
                .Setup(x => x.GetAvailableGroupBookingsAsync(_validGroupBooking, categoriesId))
                .ReturnsAsync((BookingDTO booking, List<int> categoryId) =>
                {
                    var availableBookings = BookingsTest.AvailablBookings()
                        .Where(x => x.ServiceId == booking.ServiceId
                            && x.IsAdultSlot) //because they are all categortId = 1 (adult)
                        .Take(categoriesId.Count());
                    return availableBookings;
                });

            _bookingQueriesMoq
                .Setup(x => x.FindUsersAssignedToMainUserInGroupBokingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var res = _existingGroupBookingUserMissing
                    .Where(x => x.ServiceId == 1
                        && x.IsAdultSlot && x.GroupLinkId == groupId)
                    .Select(x => x.User)
                    .ToList();

                    //var response = BookingsTest.MapToApplicationusers(res);

                    return res;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.GroupBookingAsync(_validGroupBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Select(x => x.User).Count(), 3);
        }

        #endregion
    }
}
