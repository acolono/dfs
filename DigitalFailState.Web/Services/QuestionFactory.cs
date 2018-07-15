using System.Threading;
using DigitalFailState.Web.Models;

namespace DigitalFailState.Web.Services {
    public class QuestionFactory : IQuestionProvider {
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
}