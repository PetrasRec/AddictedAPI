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
            var offers = await _context.Offer.Include(o => o.Bet).Where(o => o.User.Id == user.Id && !o.Bet.IsFinished).ToListAsync();
            return Ok(offers);
        }

        [HttpGet("{betId}")]
        public async Task<IActionResult> GetAllBetOffers(int betId)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var offers = _context.Offer
                .Include(o => o.Bet);

            return Ok(offers.Where(o => o.Bet.Id == betId).ToList());
        }

        [HttpPost("{bet_id}")]
        public async Task<IActionResult> Create(int bet_id, [FromBody] OfferModel offerData)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var offer = new Offer();
            offer.Id = 0;
            offer.Bet = _context.Bets.SingleOrDefault((b) => b.Id == bet_id);
            var _usr = await _userManager.FindByEmailAsync(User.Identity.Name);
            var user = _context.Users.Include(u => u.Coins).First(u => u.Id == _usr.Id);
            offer.User = user;
            offer.BetOptionId = offerData.BetOptionId;
            offer.Amount = offerData.Amount;

            if (offer.Amount <= 0)
            {
                return BadRequest("Invalid amount");
            }

            if (user.Coins.VaciusCoin < offer.Amount)
            {
                return BadRequest("Ure broke fam");
            }

            user.Coins.VaciusCoin -= offer.Amount;

            await _context.Offer.AddAsync(offer);
            await _context.SaveChangesAsync();
          
            return Ok(offer);
        }
    }
}
