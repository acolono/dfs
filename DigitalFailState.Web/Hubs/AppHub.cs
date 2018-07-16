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

        public AppHub(IQuestionProvider questionProvider, ScoreProvider scoreProvider, MqttService mqttService) {
            _questionProvider = questionProvider;
            _scoreProvider = scoreProvider;
            _mqttService = mqttService;
        }

        public QuestionModel GetNextQuestion() {
            return _questionProvider.GetNextQuestion();
        }

        public async Task<long> WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
            await _mqttService.UpdateScoreSilentAsync(newScore);
            await Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping() => default(int);
    }
}
