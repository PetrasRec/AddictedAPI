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

namespace Addicted.IntegrationTests
{
    public class UsersControllerTest
    {
        private UsersController usersController;
        private readonly Mock<IUsersService> _mockUsersService = new Mock<IUsersService>();
        public UsersControllerTest()
        {
            usersController = new UsersController(_mockUsersService.Object);
            usersController.ControllerContext = new ControllerContext();
            usersController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "testEmail")})
            })
            };
        }


        [Fact]
        public void GetAllUsers_ShouldReturnOk_UsersExists()
        {
            var mockedResultDto = new List<User>() { new User { Name = "TestName" } };

            _mockUsersService.Setup(x => x.GetAllUsers()).Returns(mockedResultDto);
            var response = usersController.GetAllUsers() as OkObjectResult;
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(mockedResultDto, response.Value as List<User>);
        }


        [Fact]
        public void GetUser_ShouldReturnOk_ReturnsCorrectUserBySelectingCorrectEmail()
        {
            var mockedResultDto = new User { Name = "TestName" };

            _mockUsersService.Setup(x => x.GetUserByEmail("testEmail")).Returns(mockedResultDto);
            var response = usersController.GetUser() as OkObjectResult;
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(mockedResultDto, response.Value as User);
        }

        [Fact]
        public async Task UpdateUserByID_ShouldReturnOk_UpdatesTheUser()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };

            _mockUsersService.Setup(x => x.UpdateUserByID("1", newDataDto)).ReturnsAsync(newDataDto);
            var response = await usersController.UpdateUserByID("1", newDataDto) as OkObjectResult;
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(newDataDto, response.Value as UserModel);
        }

        [Fact]
        public async Task UpdateUserByID_ShouldReturnBadRequest_NoIdProvided()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };
            var response = await usersController.UpdateUserByID(null, newDataDto);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateUserByID_ShouldReturnNotFound_ReturnsNullUser()
        {
            var newDataDto = new UserModel { Name = "NewTestName" }; 
            _mockUsersService.Setup(x => x.UpdateUserByID("1", newDataDto)).ReturnsAsync((UserModel)null);
            var response = await usersController.UpdateUserByID("1", newDataDto);
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task AddNewUser_ShouldReturnOk_UpdatesTheUser()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };

            _mockUsersService.Setup(x => x.AddNewUser(newDataDto)).ReturnsAsync(newDataDto);
            var response = await usersController.AddNewUser(newDataDto) as OkObjectResult;
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(newDataDto, response.Value as UserModel);
        }

        [Fact]
        public async Task AddNewUser_ShouldReturnBadRequest_ServiceReturnNull()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };

            _mockUsersService.Setup(x => x.AddNewUser(newDataDto)).ReturnsAsync((UserModel)null);
            var response = await usersController.AddNewUser(newDataDto);
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task RegisterNewUser_ShouldReturnOk_UpdatesTheUser()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };
            var userModelDto = new User { Name = newDataDto.Name };

            _mockUsersService.Setup(x => x.RegisterNewUser(newDataDto)).ReturnsAsync(userModelDto);
            var response = await usersController.RegisterNewUser(newDataDto) as OkObjectResult;
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(userModelDto, response.Value as User);
        }

        [Fact]
        public async Task RegisterNewUser_ShouldReturnBadRequest_ServiceReturnsNull()
        {
            var newDataDto = new UserModel { Name = "NewTestName" };

            _mockUsersService.Setup(x => x.RegisterNewUser(newDataDto)).ReturnsAsync((User)null);
            var response = await usersController.RegisterNewUser(newDataDto);
            Assert.IsType<BadRequestResult>(response);
        }
    }
}
