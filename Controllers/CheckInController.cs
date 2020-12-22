using System.Threading.Tasks;
using CheckinPPP.Data;
using CheckinPPP.Data.Entities;
using CheckinPPP.DTOs;
using CheckinPPP.Hubs;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheckinPPP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckInController(ApplicationDbContext context,
            IHubContext<PreciousPeopleHub, IPreciousPeopleClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        private IHubContext<PreciousPeopleHub, IPreciousPeopleClient> _hubContext { get; }

        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckIn data)
        {
            if (!ModelState.IsValid) return BadRequest();

            var member = ParseToMember(data);

            _context.Add(member);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var checkedInMember = ParseToCheckedInMemeberDTO(member);
                await _hubContext.Clients.All.UpdateCheckedInMembersAsync(checkedInMember);
                return Ok();
            }

            return BadRequest();
        }

        private Member ParseToMember(CheckIn checkinData)
        {
            var member = new Member
            {
                Name = checkinData.Name,
                Surname = checkinData.Surname,
                Mobile = checkinData.Mobile
            };

            return member;
        }

        private CheckedInMemberDTO ParseToCheckedInMemeberDTO(Member member)
        {
            var checkedInMember = new CheckedInMemberDTO
            {
                Name = member.Name,
                Surname = member.Surname,
                Mobile = member.Mobile
            };

            return checkedInMember;
        }
    }
}