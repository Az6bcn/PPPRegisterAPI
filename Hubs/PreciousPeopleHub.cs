using CheckinPPP.DTOs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckinPPP.Hubs
{
    public class PreciousPeopleHub: Hub<IPreciousPeopleClient>
    {

        public async Task UpdateCheckedInMemebers(CheckedInMembersDTO checkedInMembers)
        {
            await Clients.Others.UpdateCheckedInMembers(checkedInMembers);
        }
    }
}
