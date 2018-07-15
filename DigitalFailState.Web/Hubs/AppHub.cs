using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Logging;

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

        public long WrongAnswer() {
            var newScore = _scoreProvider.GetNextScore();
            Clients.Others.SendAsync("SetScore", newScore);
            return newScore;
        }

        public long GetScore() => _scoreProvider.GetScore();

        public int Ping() => default(int);
    }

    public class ScoreProvider {
        private static long _score = 0;

        public long GetNextScore() {
            var lost = new Random().Next(12,21);
            return Interlocked.Add(ref _score, 0 - lost);
        }

        public long GetScore() => Interlocked.Read(ref _score);
    }

    public class QuestionFactory {
        private int _cnt = 0;
        public QuestionModel GetNextQuestion() {
            var id = Interlocked.Increment(ref _cnt);
            return GetQuestionById(id);
        }

        public QuestionModel GetQuestionById(int id) => new QuestionModel {
            Id = id,
            Question = $"Question {id}?",
            YesConclusion = $"Conclusion {id}Y",
            NoConclusion = $"Conclusion {id}N"
        };
    }

    public class QuestionModel {
        public int Id { get; set; }
        public string Question { get; set; }
        public string YesConclusion { get; set; }
        public string NoConclusion { get; set; }
    }
}
