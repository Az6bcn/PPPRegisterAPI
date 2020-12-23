using System.Collections.Generic;

namespace CheckinPPP.DTOs
{
    public class UsersAndLinkedUsersDTO
    {
        public UsersAndLinkedUsersDTO()
        {
            LinkedUsers = new List<MemberDTO>();
        }

        public MemberDTO MainUser { get; set; }
        public List<MemberDTO> LinkedUsers { get; set; }
    }
}