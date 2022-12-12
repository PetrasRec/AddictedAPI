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

namespace Addicted.IntegrationTests
{
    public class BetsServiceApiTest
    {
        public BetsController _betsController { get; private set; }
        private readonly Mock<IBetsService> _mockBetsService = new Mock<IBetsService>();
        public BetsServiceApiTest()
        {
            _betsController = new BetsController(_mockBetsService.Object);
            _betsController.ControllerContext = new ControllerContext();
            _betsController.ControllerContext.HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "testEmail")})
            })};
        }
        
        [Fact]
        public async Task Index_ShouldReturnOk_BetsExists()
        {
            var mockedResultDto = new List<Bet>() { new Bet { Title = "title" } };

            _mockBetsService.Setup(x => x.GetAllBets()).ReturnsAsync(mockedResultDto);
            var response = (await _betsController.Index()) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);

            Assert.Equal(mockedResultDto, response.Value as List<Bet>);
        }

        [Fact]
        public async Task Index_ShouldReturnOk_EmptyBetsList()
        {
            var mockedResultDto = new List<Bet>() {};

            _mockBetsService.Setup(x => x.GetAllBets()).ReturnsAsync(mockedResultDto);
            var response = (await _betsController.Index()) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);

            Assert.Equal(mockedResultDto, response.Value as List<Bet>);
        }

        [Fact]
        public async Task GetAllActiveBets_ShouldReturnOk_ReturnsList()
        {
            var mockedResultDto = new List<Bet>() { new Bet { Title = "title" } };

            _mockBetsService.Setup(x => x.GetAllActiveBets()).ReturnsAsync(mockedResultDto);
            var response = (await _betsController.GetAllActiveBets()) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);

            Assert.Equal(mockedResultDto, response.Value as List<Bet>);
        }

        [Fact]
        public async Task Details_ShouldReturnOk_ReturnsBet()
        {
            var mockedResultDto = new Bet { Title = "title" };

            _mockBetsService.Setup(x => x.GetBetById(1)).ReturnsAsync(mockedResultDto);
            var response = (await _betsController.Details(1)) as OkObjectResult;

            Assert.IsType<OkObjectResult>(response);

            Assert.Equal(mockedResultDto, response.Value as Bet);
        }


        [Fact]
        public async Task Details_ShouldReturnBadRequest_NoSuchBet()
        {
            _mockBetsService.Setup(x => x.GetBetById(1)).ReturnsAsync((Bet)null);
            var response = (await _betsController.Details(1));

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_CreatedNewBet()
        {
            var bet = new Bet { Title = "title", Description = "description", DateStart = new DateTime(), DateEnd = new DateTime() };
            _mockBetsService.Setup(x => x.CreateBet(It.IsAny<Bet>(), It.IsAny<string>())).ReturnsAsync(bet);
            
            var response = await _betsController.Create(bet);
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_InvalidBody()
        {
            var response = await _betsController.Create(null);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task Edit_ShouldReturnOk_EditedBet()
        {
            var bet = new Bet { Title = "title", Description = "description", DateStart = new DateTime(), DateEnd = new DateTime() };
            _mockBetsService.Setup(x => x.EditBet(1, bet, "testEmail")).ReturnsAsync(bet);

            var response = await _betsController.Edit(1, bet);
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Edit_ShouldReturnOk_ErrorThrown()
        {
            var bet = new Bet { Title = "title", Description = "description", DateStart = new DateTime(), DateEnd = new DateTime() };
            _mockBetsService.Setup(x => x.EditBet(1, bet, "testEmail")).Throws(new SystemException());

            var response = await _betsController.Edit(1, bet);
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_DeletedBet()
        {
            _mockBetsService.Setup(x => x.DeleteBet(1, "testEmail")).Verifiable();

            var response = await _betsController.Delete(1);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_ErrorThrown()
        {
            var bet = new Bet { Title = "title", Description = "description", DateStart = new DateTime(), DateEnd = new DateTime() };
            _mockBetsService.Setup(x => x.DeleteBet(1, "testEmail")).Throws(new SystemException());

            var response = await _betsController.Delete(1);
            Assert.IsType<BadRequestObjectResult>(response);
        }


        [Fact]
        public async Task DeleteBetOption_ShouldReturnOk_DeletedBetOption()
        {
            _mockBetsService.Setup(x => x.DeleteBetOption(1)).Verifiable();

            var response = await _betsController.DeleteBetOption(4, 1);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task DeleteBetOption_ShouldReturnBadRequest_ErrorThrown()
        {
            _mockBetsService.Setup(x => x.DeleteBetOption(1)).Throws(new SystemException());

            var response = await _betsController.DeleteBetOption(4, 1);
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task FinishBet_ShouldReturnOk_BetFinished()
        {
            _mockBetsService.Setup(x => x.FinishBet(4, 1)).Verifiable();

            var response = await _betsController.FinishBet(4, 1);
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task FinishBet_ShouldReturnBadRequest_ErrorThrown()
        {
            _mockBetsService.Setup(x => x.FinishBet(4, 1)).Throws(new SystemException());

            var response = await _betsController.FinishBet(4, 1);
            Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
