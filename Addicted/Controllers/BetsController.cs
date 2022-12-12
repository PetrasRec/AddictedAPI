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
using Addicted.Service;

namespace Addicted.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    [Route("[controller]")]
    public class BetsController : Controller
    {
        private readonly IBetsService _betsService;

        public BetsController(IBetsService betsService)
        {
            _betsService = betsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var bets = await _betsService.GetAllBets();
                return Ok(bets);
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }     
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveBets()
        {
            try
            {
                var bets = await _betsService.GetAllActiveBets();
                return Ok(bets);
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var bet = await _betsService.GetBetById(id.Value);
                if (bet == null)
                {
                    return NotFound();
                }

                return Ok(bet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bet bet)
        {
            try
            {
                if (!ModelState.IsValid || bet == null)
                    return BadRequest();

                var createdBet = await _betsService.CreateBet(bet, User.Identity.Name);
                return Ok(createdBet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Bet modifiedBet)
        {
            try
            {
                var updatedBet = await _betsService.EditBet(id, modifiedBet, User.Identity.Name);
                return Ok(updatedBet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _betsService.DeleteBet(id, User.Identity.Name);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}/options/{option_id}")]
        public async Task<IActionResult> DeleteBetOption(int id, int optionId)
        {
            try
            {
                await _betsService.DeleteBetOption(optionId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{bet_id}/finish/{option_id}")]
        public async Task<IActionResult> FinishBet(int bet_id, int option_id)
        {
            try
            {
                await _betsService.FinishBet(bet_id, option_id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

