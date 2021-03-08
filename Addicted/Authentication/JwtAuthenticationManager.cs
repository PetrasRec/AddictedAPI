using Addicted.Service;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Addicted.Authentication
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string JwtAccessKey = "random asdasdaaaaaaaaaaaaaaaaaaakey ";
        private readonly IUsersService usersService;

        public JwtAuthenticationManager(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        public async Task<JwtToken> Authenticate(string email, string password)
        {
            bool correctLogInDetails = await usersService.LogInUser(email, password);
            if (!correctLogInDetails)
            {
                return null;
            }
            var user = usersService.GetUserByEmail(email);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(this.JwtAccessKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, user.Id),
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtToken() { Token = tokenHandler.WriteToken(token) };
        }
    }
}
