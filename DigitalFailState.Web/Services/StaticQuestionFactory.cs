using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DigitalFailState.Web.Models;

namespace DigitalFailState.Web.Services
{
    public class StaticQuestionFactory : IQuestionProvider
    {
        private static readonly object Sync = new object();
        private static int _questionId = 0;
        private static readonly ImmutableList<QuestionModel> Questions = StaticQuestionFactory.Init();

        private static ImmutableList<QuestionModel> Init() {
            // TODO: read from csv/etc...
            var qf = new QuestionFactory();
            return Enumerable.Range(1, 20).Select(qf.GetQuestionById).ToImmutableList();
        }

        public QuestionModel GetNextQuestion() {
            lock (Sync) {
                _questionId++;
                if (_questionId >= 20) _questionId = 1;
                return GetQuestionById(_questionId);
            }
        }

        public QuestionModel GetQuestionById(int id) {
            lock (Sync) {
                return Questions.Single(l => l.Id == id);
            }
        }
    }
}
