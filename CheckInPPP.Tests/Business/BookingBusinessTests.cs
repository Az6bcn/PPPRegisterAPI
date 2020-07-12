using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckinPPP.Business;
using CheckinPPP.Data.Entities;
using CheckinPPP.Data.Queries;
using CheckinPPP.DTOs;
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
        private IEnumerable<Booking> _existingGroupBookingMemberMissing;
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
            _existingGroupBookingMemberMissing = BookingsTest.ExistingGroupBookingsLastMemberMissing(groupId);
        }

        #region Single Booking

        [TestMethod]
        public async Task SingleBookingWithExistingMemberInDb_WhenCalledWithValidBooking_ShouldReturnObjectWithIdAndMemberId()
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
                .Setup(x => x.FindMemberByEmailAsync(It.IsAny<string>(), It.IsAny<MemberDTO>()))
                .ReturnsAsync((string email, MemberDTO member) =>
                {
                    return BookingsTest.MapToBooking(_validSingleBooking).Member;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.SingleBookingAsync(_validSingleBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 1);
            Assert.AreNotEqual(Guid.Empty, result.BookingReference);
            Assert.IsNotNull(result.MemberId);
            Assert.IsNull(result.Member);
        }


        [TestMethod]
        public async Task SingleBookingWithNoneExistingMemberIndB_WhenCalledWithValidBooking_ShouldReturnObjectWithIdAndMember()
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
                .Setup(x => x.FindMemberByEmailAsync(It.IsAny<string>(), It.IsAny<MemberDTO>()))
                .ReturnsAsync((string email, MemberDTO member) =>
                {
                    return null;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.SingleBookingAsync(_validSingleBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, 1);
            Assert.AreNotEqual(Guid.Empty, result.BookingReference);
            Assert.IsNotNull(result.Member);
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
        public async Task GroupBookingWithExistingMemberInDb_WhenCalled_ShouldReturnObjectWithIdAndMembers()
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
                .Setup(x => x.FindMembersOfGroupBookingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var res = _existingGroupBooking
                    .Where(x => x.ServiceId == 1
                        && x.IsAdultSlot && x.GroupLinkId == groupId)
                    .Select(x => x.Member)
                    .ToList();

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
        public async Task GroupBookingWithNoneExistingMemberInDb_WhenCalled_ShouldReturnObjectWithIdAndAllMembers()
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
                .Setup(x => x.FindMembersOfGroupBookingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    return new List<Member>();
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.GroupBookingAsync(_validGroupBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Select(x => x.Member).Count(), 3);
            Assert.AreNotEqual(result.Select(x => x.BookingReference), Guid.Empty);
            Assert.AreNotEqual(result.Select(x => x.BookingReference), null);
        }


        [TestMethod]
        public async Task GroupBookingWithSomeExistingMemberInDb_WhenCalled_ShouldReturnObjectWithIdAndAllMembers()
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
                .Setup(x => x.FindMembersOfGroupBookingByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    var res = _existingGroupBookingMemberMissing
                    .Where(x => x.ServiceId == 1
                        && x.IsAdultSlot && x.GroupLinkId == groupId)
                    .Select(x => x.Member)
                    .ToList();

                    return res;
                });

            var bookingBusiness = new BookingBusiness(_bookingQueriesMoq.Object);

            // Act
            var result = await bookingBusiness.GroupBookingAsync(_validGroupBooking);

            // Asset
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Select(x => x.Member).Count(), 3);
        }

        #endregion
    }
}
