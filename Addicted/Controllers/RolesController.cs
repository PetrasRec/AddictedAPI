using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        
        public RolesController(RoleManager<IdentityRole> _roleManager)
        {
            this.roleManager = _roleManager;
        }

        [HttpGet]
        public IActionResult GetAllRoles()
        {
            return Ok(roleManager.Roles.ToList());
        }
    }
}
