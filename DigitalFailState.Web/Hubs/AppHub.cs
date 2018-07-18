using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalFailState.Web.Models;
using DigitalFailState.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Hubs
{

    public class AppHub : Hub
    {
        private readonly IQuestionProvider _questionProvider;
        private readonly ScoreProvider _scoreProvider;
        private readonly MqttService _mqttService;
        private static readonly IDictionary<string, int> ActiveQuestions = new Dictionary<string, int>();

        public AppHub(IQuestionProvider questionProvider, ScoreProvider scoreProvider, MqttService mqttService) {
            _questionProvider = questionProvider;
            _scoreProvider = scoreProvider;
            _mqttService = mqttService;
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
            await _mqttService.UpdateScoreSilentAsync(newScore);
            await Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping() => default(int);

        public override Task OnDisconnectedAsync(Exception exception) {
            lock (ActiveQuestions) {
                ActiveQuestions.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
