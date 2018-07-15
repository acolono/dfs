using DigitalFailState.Web.Models;
using DigitalFailState.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Hubs
{

    public class AppHub : Hub
    {
        private readonly IQuestionProvider _questionProvider;
        private readonly ScoreProvider _scoreProvider;

        public AppHub(IQuestionProvider questionProvider, ScoreProvider scoreProvider) {
            _questionProvider = questionProvider;
            _scoreProvider = scoreProvider;
        }

        public QuestionModel GetNextQuestion() {
            return _questionProvider.GetNextQuestion();
        }

        public long WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
            Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping() => default(int);
    }
}
