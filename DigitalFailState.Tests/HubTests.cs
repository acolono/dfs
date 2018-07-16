using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private static HubConnection GetHub(HttpMessageHandler handler, Uri url) {

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
            var server = HttpTests.GetServer();
            var handler = server.CreateHandler();
            var client = server.CreateClient();
            var hubUrl = new Uri("http://test/appHub");

            var conn = GetHub(handler, hubUrl);
            var connOther = GetHub(handler, hubUrl);
            try {

                long? otherScore = null;
                connOther.On<long>("SetScore", s => { otherScore = s; });

                await connOther.StartAsync();
                await conn.StartAsync();
                
                var pong = await conn.InvokeAsync<int>("Ping");
                Assert.IsTrue(pong == default(int));

                var next = await conn.InvokeAsync<QuestionModel>("GetNextQuestion");
                TestContext.WriteLine($"next question: {next.Question}");
                Assert.IsTrue(next.Question.Length > 0);

                var score1 = await conn.InvokeAsync<long>("GetScore");
                var score2 = await conn.InvokeAsync<long>("WrongAnswer");

                Assert.IsTrue(score1 > score2 && score2 < 0);

                var legacyScoreString = await client.GetStringAsync("/score/legacy");
                Assert.IsTrue(legacyScoreString.Contains($"{score2}"));

                await Task.Delay(500);
                Assert.IsTrue(otherScore.HasValue && otherScore == score2);

            } finally {
                await conn.DisposeAsync();
                await connOther.DisposeAsync();
            }
        }
    }
}
