using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Hubs
{

    public class AppHub : Hub
    {
        private readonly QuestionFactory _questionFactory;
        private readonly ScoreProvider _scoreProvider;

        public AppHub(QuestionFactory questionFactory, ScoreProvider scoreProvider) {
            _questionFactory = questionFactory;
            _scoreProvider = scoreProvider;
        }
        public QuestionModel GetNextQuestion() {
            return _questionFactory.GetNextQuestion();
        }

        public void WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
            Clients.All.SendAsync("SetScore", newScore);
        }

        public override Task OnConnectedAsync() {
            var score = _scoreProvider.GetScore();
            Clients.All.SendAsync("SetScore", score);
            return base.OnConnectedAsync();
        }
    }

    public class ScoreProvider {
        private long _score = 0;

        public long GetNextScore() {
            var lost = new Random().Next(12,21);
            return Interlocked.Add(ref _score, 0 - lost);
        }

        public long GetScore() => _score;
    }

    public class QuestionFactory {
        private int _cnt = 0;
        public QuestionModel GetNextQuestion() {
            var id = Interlocked.Increment(ref _cnt);
            return new QuestionModel {
                Question = $"Question {id}?",
                YesConclusion = $"Conclusion {id}Y",
                NoConclusion = $"Conclusion {id}N"
            };
        }
    }

    public class QuestionModel {
        public string Question { get; set; }
        public string YesConclusion { get; set; }
        public string NoConclusion { get; set; }
    }
}
