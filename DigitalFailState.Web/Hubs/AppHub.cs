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

        public AppHub(IQuestionProvider questionProvider, ScoreProvider scoreProvider, MqttService mqttService) {
            _questionProvider = questionProvider;
            _scoreProvider = scoreProvider;
            _mqttService = mqttService;
        }

        public QuestionModel GetNextQuestion() {
            return _questionProvider.GetNextQuestion();
        }

        public long WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
#pragma warning disable 4014 // we dont (a)wait because we dont care...
            _mqttService.UpdateScoreSilentAsync(newScore);
#pragma warning restore 4014
            Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping() => default(int);
    }
}
