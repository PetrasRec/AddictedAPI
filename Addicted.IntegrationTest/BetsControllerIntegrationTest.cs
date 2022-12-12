using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Addicted.Controllers;
using Addicted.Models;
using Xunit;
using Moq;
using Addicted.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Addicted.IntegrationTests
{
    public class BetsControllerIntegrationTest
    {
        public class Controller
        {
            public BetsController BetsController { get; set; }
            public AuthenticationContext Context { get; set; }
        }


        public DbContextOptions<AuthenticationContext> GetOptions()
        {
            return new DbContextOptionsBuilder<AuthenticationContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
        }
        public async Task<AuthenticationContext> GetAuthenticationContext(List<Bet> bets)
        {
            var options = GetOptions();
            var databaseContext = new AuthenticationContext(options);
            databaseContext.Database.EnsureCreated();

            foreach (var bet in bets)
            {
                databaseContext.Bets.Add(bet);
                await databaseContext.SaveChangesAsync();
            }

            databaseContext.Users.Add(new User() { Id = "1", Name = "test", Email = "testEmail" });
            await databaseContext.SaveChangesAsync();
            return databaseContext;
        }

        public async Task<Controller> GetBetsController(List<Bet> bets)
        {
            var context = await GetAuthenticationContext(bets);
            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);

            var controller = new BetsController(new BetsService(context, userManager));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "testEmail")})
            })
            };

            return new Controller { BetsController = controller, Context = context };
        }

        public BetsControllerIntegrationTest()
        {
          
        }

        [Fact]
        public async Task GetAllActiveBets_ShouldReturnOk_BetsExists()
        {
            var mockedResultDto = new List<Bet>() { 
                new Bet { 
                    Id = 1,
                    Title = "test", 
                    BetOptions = new List<BetOption>() { new BetOption { Id=1} }
                } 
            };

            var controller = new BetsController(
                new BetsService(
                    await GetAuthenticationContext(mockedResultDto), null
                )
            );

            var response = (await controller.GetAllActiveBets()) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);
            var value = response.Value as List<Bet>;
            Assert.Equal(JsonConvert.SerializeObject(mockedResultDto), JsonConvert.SerializeObject(value));
        }

        [Fact]
        public async Task GetAllActiveBets_ShouldReturnOk_BetIsFinishedShouldReturnEmpty()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    IsFinished = true,
                    BetOptions = new List<BetOption>() { new BetOption { Id=1} }
                }
            };

            var controller = new BetsController(
                new BetsService(
                    await GetAuthenticationContext(mockedResultDto), null
                )
            );

            var response = (await controller.GetAllActiveBets()) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);
            var value = response.Value as List<Bet>;
            Assert.Equal("[]", JsonConvert.SerializeObject(value));
        }

        [Fact]
        public async Task Details_ShouldReturnOk_BetExists()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    BetOptions = new List<BetOption>() { },
                }
            };

            var controller = new BetsController(
                new BetsService(
                    await GetAuthenticationContext(mockedResultDto), null
                )
            );

            var response = (await controller.Details(1)) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);
            var value = response.Value as Bet;
            Assert.Equal(JsonConvert.SerializeObject(mockedResultDto[0]), JsonConvert.SerializeObject(value));
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_BetDoesNotExists()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    BetOptions = new List<BetOption>() { },
                }
            };

            var controller = new BetsController(
                new BetsService(
                    await GetAuthenticationContext(mockedResultDto), null
                )
            );

            var response = (await controller.Details(2));

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_CorrectBetInformation()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    BetOptions = new List<BetOption>() { },
                }
            };
            var controller = await GetBetsController(mockedResultDto);

            var addBet = new Bet
            {
                Id = 2,
                Title = "test",
                BetOptions = new List<BetOption>() { },
            };

            var response = (await controller.BetsController.Create(addBet));

            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(addBet, controller.Context.Bets.Single(b => b.Id == 2));
        }

        [Fact]
        public async Task Edit_ShouldReturnOk_CorrectBetInformation()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    BetOptions = new List<BetOption>() { },
                }
            };
            var controller = await GetBetsController(mockedResultDto);

            var addBet = new Bet
            {
                Id = 1,
                Title = "test2",
                BetOptions = new List<BetOption>() { },
            };

            var response = (await controller.BetsController.Edit(1, addBet));

            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Edit_ShouldReturnBadRequest_InvalidId()
        {
            var mockedResultDto = new List<Bet>() {
                new Bet {
                    Id = 1,
                    Title = "test",
                    BetOptions = new List<BetOption>() { },
                }
            };
            var controller = await GetBetsController(mockedResultDto);

            var addBet = new Bet
            {
                Id = 1,
                Title = "test2",
                BetOptions = new List<BetOption>() { },
            };

            var response = (await controller.BetsController.Edit(5, addBet));

            Assert.IsType<OkObjectResult>(response);
            Assert.Null((response as OkObjectResult).Value);
        }

    }
}
