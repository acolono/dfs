using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using DigitalFailState.Web;
using DigitalFailState.Web.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace DigitalFailState.Tests
{
    [TestFixture]
    public class HubTests
    {
        private static TestServer GetServer() {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            return new TestServer(builder);
        }
        private static HubConnection GetHub(Uri url) {

            var handler = GetServer().CreateHandler();

            var hubConnection = new HubConnectionBuilder()
                               .WithUrl(url, HttpTransportType.LongPolling, o => {
                                   o.Transports = HttpTransportType.LongPolling;
                                   o.Url = url;
                                   o.HttpMessageHandlerFactory = mh => handler;
                               })
                               .Build();
            return hubConnection;
        }

        [Test]
        public async Task WrongAnswerDecreasesScore() {
            var conn = GetHub(new Uri("http://test/appHub"));
            try {

                //TODO: callbacks with testhost
                //long? score = null;
                //conn.On<long>("SetScore", s => { score = s; });

                await conn.StartAsync();
                
                var pong = await conn.InvokeAsync<int>("Ping");
                Assert.IsTrue(pong == default(int));

                var next = await conn.InvokeAsync<QuestionModel>("GetNextQuestion");
                TestContext.WriteLine($"next question: {next.Question}");
                Assert.IsTrue(next.Question.Length > 0);

                var score1 = await conn.InvokeAsync<long>("GetScore");
                var score2 = await conn.InvokeAsync<long>("WrongAnswer");

                Assert.IsTrue(score1 > score2 && score2 < 0);

                //await Task.Delay(1000);
                //Assert.IsTrue(score.HasValue && score == score2);

            } finally {
                await conn.DisposeAsync();
            }
        }
    }
}
