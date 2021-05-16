using System;
using System.Net;
using Addicted.Models;
using Xunit;

namespace Addicted.IntegrationTests
{
    public class OffersApiIntegrationTest
    {
        public TestClientProvider TestClientProvider { get; private set; }
        public OffersApiIntegrationTest()
        {
            TestClientProvider = new TestClientProvider();
        }

        [Fact]
        public void TestGetAllOffers()
        {
            var response = TestClientProvider.Get("/offers");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public void TestCreateOffer()
        {
            var offer = new Offer
            {
                Amount = 100,
                Bet = new Bet
                {
                    BetOptions = null,
                    DateEnd = DateTime.Now,
                    DateStart = DateTime.Now,
                    Description = "Desc",
                    Title = "Title",
                    User = Utils.GenerateUser("test")
                },
                BetOptionId = 1,
                User = Utils.GenerateUser("test")
            };

            var res = TestClientProvider.Post("/offers/create", offer);
            res.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal(res.Data.Amount, 100);
        }
    }
}
