using System.Collections.Generic;
using System.Security.Claims;

namespace CheckinPPP.Business
{
    public interface IJwtFactory
    {
        string GenerateToken(List<Claim> userClaims = null);
    }
}