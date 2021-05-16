using Addicted.Authentication;
using Addicted.Models;
using Addicted.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private IUsersService usersService;
        public AuthenticateController(IJwtAuthenticationManager jwtAuthenticationManager, IUsersService usersService)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserCredentials userCred)
        {
            var token = await jwtAuthenticationManager.Authenticate(userCred.Email, userCred.Password);
            if (token == null)
            {
                return Unauthorized("Wrong login details. Email or password is incorrect");
            }

            var role = await usersService.GetUserRoleName(userCred.Email);

            Response.Cookies.Append("access_token", token.Token, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
            }); 

            return Ok(new Dictionary<String, String>() { ["roleName"] = role });
        }
    }
}
