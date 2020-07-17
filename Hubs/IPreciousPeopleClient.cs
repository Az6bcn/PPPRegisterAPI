﻿using CheckinPPP.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.Hubs
{
    public interface IPreciousPeopleClient
    {
        // methods that will be called on the client, client must implement this methods
        Task UpdateCheckedInMembersAsync(CheckedInMemberDTO checkedInMembers);
        Task UpdateSlotsAvailableAsync(DateTime date);
        Task ReceivedBookingsUpdateAsync(BookingsUpdateSignalR availableBookings);
        Task ReceivedBookingsToSignInUpdateAsync(List<CheckedInMemberDTO> bookings);
    }
}
