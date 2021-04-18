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
        public ActionResult<dynamic> getAllUsers()
        {
            var users = usersService.GetAllUsers();
            return Ok(users);
        }


        [HttpPost]
        public async Task<dynamic> RegisterNewUser([FromBody] UserModel user)
        {
            var addedUser = await usersService.RegisterNewUser(user);
            return Ok(addedUser);
        }
    }
}
