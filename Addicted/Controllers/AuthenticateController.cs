using Addicted.Authentication;
using Addicted.Models;
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

        public AuthenticateController(IJwtAuthenticationManager jwtAuthenticationManager)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserCredentials userCred)
        {
            var token = await jwtAuthenticationManager.Authenticate(userCred.Email, userCred.Password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
