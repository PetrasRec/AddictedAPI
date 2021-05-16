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
    public class BetsController : Controller
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public BetsController(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var bets = await _context.Bets.Include(b => b.BetOptions).Include(b => b.User).ToListAsync();
            return Ok(bets);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveBets()
        {
            var bets = await _context.Bets
                .Include(b => b.BetOptions)
                .Include(b => b.User)
                .Where(b => !b.IsFinished && b.BetOptions.Count > 0)
                .ToListAsync();

            return Ok(bets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bet = await _context.Bets.Include(p => p.BetOptions).FirstOrDefaultAsync(m => m.Id == id);
            if (bet == null)
            {
                return NotFound();
            }

            return Ok(bet);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bet bet)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            bet.User = await _userManager.FindByEmailAsync(User.Identity.Name);

            await _context.Bets.AddAsync(bet);
            await _context.SaveChangesAsync();
          
            return Ok(bet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Bet modifiedBet)
        {
            if (!ModelState.IsValid) return BadRequest();

            
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var bet = await _context.Bets.FindAsync(id);

            if (bet == null)
            {
                return NotFound();
            }

           // if (user.Id != bet?.User?.Id)
           // {
           //     return Unauthorized();
           // }

            bet.Title = modifiedBet.Title;
            bet.Description = modifiedBet.Description;
            bet.BetOptions = modifiedBet.BetOptions;
            bet.DateStart = modifiedBet.DateStart;
            bet.DateEnd = modifiedBet.DateEnd;
            await _context.SaveChangesAsync();

            return Ok(bet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bet = await _context.Bets.FindAsync(id);

            if (bet == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (bet.User?.Id != user.Id)
            {
                return Unauthorized();
            }

            _context.Bets.Remove(bet);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/options/{option_id}")]
        public async Task<IActionResult> DeleteBetOption(int id, int optionId)
        {
            var bet = await _context.Bets.FindAsync(id);
            
           // _context.Bets.Remove(bet.BetOptions.Single(o => o.Id == optionId));
           //  var betOption = await _context.BetOptions.FindAsync(optionId);
           //  _context.BetOptions.Remove(betOption);
            return Ok();
        }

        [HttpPost("{bet_id}/finish/{option_id}")]
        public async Task<IActionResult> FinishBet(int bet_id, int option_id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var bet = _context.Bets.Include(bet => bet.BetOptions).SingleOrDefault(bet => bet.Id == bet_id);
            if (bet.IsFinished)
            {
                return Conflict();
            }
            bet.IsFinished = true;
            var winnerBet = bet.BetOptions.SingleOrDefault(option => option.Id == option_id);
            winnerBet.IsWinner = true;

            var offers = _context.Offer;
            var winners = _context.Offer
                            .Include(o => o.User)
                            .Include(o => o.Bet)
                            .Where(o => o.Bet.Id == bet_id && o.BetOptionId == option_id)
                            .ToList();

            var totalAmount = _context.Offer
                                .Where(o => o.Bet.Id == bet_id)
                                .Sum(o => o.Amount);

            var winnerAmount = _context.Offer
                                .Where(o => o.Bet.Id == bet_id && o.BetOptionId == option_id)
                                .Sum(o => o.Amount);

            foreach (var winner in winners)
            {
                int winnings = (int)((double) winner.Amount / (double) winnerAmount * (double) totalAmount);

                var _usr = await _userManager.FindByEmailAsync(winner.User.Email);
                var user = _context.Users.Include(u => u.Coins).First(u => u.Id == _usr.Id);

                user.Coins.VaciusCoin += winnings;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

