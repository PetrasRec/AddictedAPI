using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addicted.Service;
using Microsoft.AspNetCore.Authorization;
using Addicted.Models;
using Addicted.Config;

namespace Addicted.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Authorize()]
        public IActionResult GetAllUsers()
        {
            return Ok(usersService.GetAllUsers());
        }

        [HttpGet("profile")]
        [Authorize()]
        public IActionResult GetUser()
        {
            var user = usersService.GetUserByEmail(User.Identity.Name);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize()]
        public async Task<IActionResult> UpdateUserByID(string ?id, [FromBody] UserModel newData)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var updatedUser = await usersService.UpdateUserByID(id, newData);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> AddNewUser([FromBody] UserModel user)
        {
            var addedUser = await usersService.AddNewUser(user);
            if (addedUser == null)
            {
                return BadRequest();
            }

            return Ok(addedUser);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserModel user)
        {
            var addedUser = await usersService.RegisterNewUser(user);
            if (addedUser == null)
            {
                return BadRequest();
            }
            return Ok(addedUser);
        }
    }
}
