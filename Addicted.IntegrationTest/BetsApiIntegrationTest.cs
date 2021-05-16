using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Addicted.Models;
using Xunit;

namespace Addicted.IntegrationTests
{
    public class BetsApiIntegrationTest
    {
        public TestClientProvider TestClientProvider { get; private set; }
        public BetsApiIntegrationTest()
        {
            TestClientProvider = new TestClientProvider();
        }

        [Fact]
        public void TestGetAllBets()
        {
            var response = TestClientProvider.Get("/bets");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public void TestCreateBet()
        {
            var bet = new Bet
            {
                BetOptions = null,
                DateEnd = DateTime.Now,
                DateStart = DateTime.Now,
                Description = "Desc",
                Title = "Title",
                User = Utils.GenerateUser("test")
            };

            var res = TestClientProvider.Post("/bets/create", bet);
            res.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal(res.Data.User.Name, "test");
        }

        [Fact]
        public void TestEditBet()
        {
            var user = Utils.GenerateUser("test");

            var betInfo = new Bet {BetOptions = null, DateEnd = DateTime.Now, DateStart = DateTime.Now,
                Description = "created", Title = "created",Id = 1, User = Utils.GenerateUser("test") };

            var createRes = TestClientProvider.Post("/bets/create", betInfo);
            createRes.EnsureSuccessStatusCode();

            betInfo.Description = "updated";
            betInfo.Title = "updated";
            var updateRes = TestClientProvider.Post($"/bets/edit/{createRes.Data.Id}", betInfo);
            updateRes.EnsureSuccessStatusCode();

            Assert.Equal(updateRes.Data.Description, "updated");
            Assert.Equal(updateRes.Data.Title, "updated");

        }
    }
}
