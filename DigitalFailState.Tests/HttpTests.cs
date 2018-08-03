using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalFailState.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace DigitalFailState.Tests
{
    [TestFixture]
    public class HttpTests
    {
        internal static TestServer GetServer() {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            return new TestServer(builder);
        }

        [TestCase("/")]
        [TestCase("/app")]
        [TestCase("/score")]
        [TestCase("/score/legacy")]
        public async Task CanServe(string url) {
            var client = GetServer().CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
