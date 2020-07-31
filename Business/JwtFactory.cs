using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CheckinPPP.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CheckinPPP.Business
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtOptions _jwtOptions;

        public JwtFactory(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }


        public string GenerateToken(List<Claim> userClaims)
        {
            var tokenOptions = GetJwtSecurityTokenOptions(userClaims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }


        private JwtSecurityToken GetJwtSecurityTokenOptions(List<Claim> userClaims)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: userClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: GetSigningCredentials()
                );

            return token;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);

            var siginigCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            return siginigCredentials;
        }
    }
}