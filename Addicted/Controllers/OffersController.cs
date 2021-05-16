using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Addicted.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Addicted.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    [Route("[controller]")]
    public class OffersController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public OffersController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var offers = await _context.Offer.Include(o => o.Bet).Where(o => o.User.Id == user.Id).ToListAsync();
            return Ok(offers);
        }

        [HttpPost("{bet_id}")]
        public async Task<IActionResult> Create(int bet_id, [FromBody] Offer offer)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            offer.Id = 0;

            var _usr = await _userManager.FindByEmailAsync(User.Identity.Name);
            var user = _context.Users.Include(u => u.Coins).First(u => u.Id == _usr.Id);

            if (offer.Amount <= 0)
            {
                return BadRequest("Invalid amount");
            }

            if (user.Coins.VaciusCoin < offer.Amount)
            {
                return BadRequest("Ure broke fam");
            }

            offer.User = user;
            offer.Bet = await _context.Bets.FindAsync(bet_id);

            user.Coins.VaciusCoin -= offer.Amount;

            await _context.Offer.AddAsync(offer);
            await _context.SaveChangesAsync();
          
            return Ok(offer);
        }
    }
}
