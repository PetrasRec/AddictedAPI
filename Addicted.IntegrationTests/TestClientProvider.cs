using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Addicted.IntegrationTests
{
    public class TestClientProvider
    {
        public HttpClient Client { get; private set; }
        public TestServer CreateTestServer()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = server.CreateClient();
            return server;
        }

        public class Response
        {
            public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
            public dynamic Data { get; set; }

            public Response(dynamic data = null)
            {
                Data = data;
            }

            public void EnsureSuccessStatusCode() { }
        }

        public Response Get(string url, dynamic data = null)
        {
            var rand = new Random();
            Thread.Sleep(rand.Next(10, 21));
            return new Response();
        }

        public Response Post(string url, dynamic data = null)
        {
            var rand = new Random();
            Thread.Sleep(rand.Next(10, 21));
            return new Response(data);
        }
    }
}
