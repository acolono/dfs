using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DigitalFailState.Web.Models;
using DigitalFailState.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DigitalFailState.Web.Hubs
{

    public class AppHub : Hub
    {
        private readonly IQuestionProvider _questionProvider;
        private readonly ScoreProvider _scoreProvider;

        private static readonly IDictionary<string, int> ActiveQuestions = new Dictionary<string, int>();
        private static long _lastClientActivity = DateTime.Now.Ticks;

        public AppHub(IQuestionProvider questionProvider, ScoreProvider scoreProvider) {
            _questionProvider = questionProvider;
            _scoreProvider = scoreProvider;
        }

        public QuestionModel GetNextQuestion() {
            List<int> activeIds;
            lock (ActiveQuestions) {
                activeIds = ActiveQuestions.Select(q => q.Value).ToList();
            }

            var next = _questionProvider.GetNextQuestion();

            for (var i = 0; i < 999; i++) {
                if(!activeIds.Contains(next.Id)) break;
                next = _questionProvider.GetNextQuestion();
            }

            lock (ActiveQuestions) {
                ActiveQuestions[Context.ConnectionId] = next.Id;
            }
            
            return next;
        }

        public async Task<long> WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
            await Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping(int questionId = -1) {
            ClientActivity();

            if (questionId > 0) {
                lock (ActiveQuestions) {
                    ActiveQuestions[Context.ConnectionId] = questionId;
                }
            }

            return default(int);
        }

        public static void ClientActivity() {
            Interlocked.Exchange(ref _lastClientActivity, DateTime.Now.Ticks);
        }

        public static (DateTime lastActivity, TimeSpan inactiveTimeSpan, string clientInfo, string questionsJson) GetLastClientActivity() {
            var ticks = Interlocked.Read(ref _lastClientActivity);
            var ts = new DateTime(ticks);
            var diff = DateTime.Now - ts;
            var ci = string.Empty;
            string questions;
            lock (ActiveQuestions) {
                var clientCount = ActiveQuestions.Count;
                if (clientCount > 0) {
                    ci = $"{clientCount}:{string.Join(",", ActiveQuestions.Select(q => q.Value))}";
                }

                var t = ActiveQuestions.Select(q => new {connectionId = q.Key, questionId = q.Value});
                questions = JToken.FromObject(t).ToString(Formatting.Indented);
            }
            return (ts, diff, ci, questions);
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            lock (ActiveQuestions) {
                ActiveQuestions.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync() {
            ClientActivity();
            lock (ActiveQuestions) {
                if (!ActiveQuestions.ContainsKey(Context.ConnectionId)) {
                    ActiveQuestions.Add(Context.ConnectionId, -1);
                }
            }
            return base.OnConnectedAsync();
        }
    }
}
