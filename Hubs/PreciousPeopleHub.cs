using CheckinPPP.DTOs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.Hubs
{
    // methods that will be called by the client.
    public class PreciousPeopleHub : Hub<IPreciousPeopleClient>
    {

        public async Task UpdateCheckedInMemebers(CheckedInMemberDTO checkedInMembers)
        {
            await Clients.Others.UpdateCheckedInMembers(checkedInMembers);
        }
    }
}
