using Addicted.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Addicted.Service
{
    public interface IBetsService
    {
        public Task<List<Bet>> GetAllBets();
        public Task<List<Bet>> GetAllActiveBets();
        public Task<Bet> GetBetById(int id);
        public Task<Bet> CreateBet(Bet bet, string creatorEmail);
        public Task<Bet> EditBet(int id, Bet modifiedBet, string callerEmail);
        public Task DeleteBet(int id, string callerEmail);
        public Task DeleteBetOption(int betOptionId);
        public Task FinishBet(int bet_id, int option_id);
    }

    public class BetsService : IBetsService
    {
        private readonly AuthenticationContext _context;
        private readonly UserManager<User> _userManager;

        public BetsService(AuthenticationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Bet>> GetAllBets()
        {
            var bets = await _context.Bets.Include(b => b.BetOptions).Include(b => b.User).ToListAsync();
            return bets;
        }

        public async Task<List<Bet>> GetAllActiveBets()
        {
            var bets = await _context.Bets
              .Include(b => b.BetOptions)
              .Include(b => b.User)
              .Where(b => !b.IsFinished && b.BetOptions.Count > 0)
              .ToListAsync();

            return bets;
        }

        public async Task<Bet> GetBetById(int id)
        {
            var bet = await _context.Bets.Include(p => p.BetOptions).FirstOrDefaultAsync(m => m.Id == id);
            return bet;
        }

        public async Task<Bet> CreateBet(Bet bet, string creatorEmail)
        {
            bet.User = await _userManager.FindByEmailAsync(creatorEmail);

            await _context.Bets.AddAsync(bet);
            await _context.SaveChangesAsync();
            return bet;
        }

        public async Task<Bet> EditBet(int id, Bet modifiedBet, string callerEmail)
        {
            var user = await _userManager.FindByEmailAsync(callerEmail);
            var bet = await _context.Bets.FindAsync(id);

            if (bet == null)
            {
                return null;
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
            return bet;
        }

        public async Task DeleteBet(int id, string callerEmail)
        {
            var bet = await _context.Bets.FindAsync(id);

            if (bet == null)
            {
                throw new System.Exception("Invalid Id");
            }

            var user = await _userManager.FindByEmailAsync(callerEmail);

            if (bet.User?.Id != user.Id)
            {
                throw new System.Exception("Not owner of the bet");
            }

            _context.Bets.Remove(bet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBetOption(int betOptionId)
        {
            var betOption = await _context.BetOption.FindAsync(betOptionId);
            if (betOption == null)
            {
                throw new System.Exception("Invalid id");
            }

            _context.BetOption.Remove(betOption);
            await _context.SaveChangesAsync();
        }

        public async Task FinishBet(int bet_id, int option_id)
        {
            var bet = _context.Bets.Include(bet => bet.BetOptions).SingleOrDefault(bet => bet.Id == bet_id);
            if (bet.IsFinished)
            {
                throw new System.Exception("Bet already finished");
            }

            bet.IsFinished = true;
            var winnerBet = bet.BetOptions.SingleOrDefault(option => option.Id == option_id);
            winnerBet.IsWinner = true;

            var offers = _context.Offer
                            .Include(o => o.User)
                            .Include(o => o.Bet)
                            .ToList();
            var winners = _context.Offer
                            .Include(o => o.User)
                            .Include(o => o.Bet)
                            .Where(o => o.BetOptionId == option_id)
                            .ToList();

            var totalAmount = _context.Offer
                                .Where(o => o.Bet.Id == bet_id)
                                .Sum(o => o.Amount);

            var winnerAmount = _context.Offer
                                .Where(o => o.Bet.Id == bet_id && o.BetOptionId == option_id)
                                .Sum(o => o.Amount);

            foreach (var winner in winners)
            {
                int winnings = (int)((double)winner.Amount / (double)winnerAmount * (double)totalAmount);

                var _usr = await _userManager.FindByEmailAsync(winner.User.Email);
                var user = _context.Users.Include(u => u.Coins).First(u => u.Id == _usr.Id);

                user.Coins.VaciusCoin += winnings;
            }

            await _context.SaveChangesAsync();
        }

    }
}
