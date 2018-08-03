using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalFailState.Web.Services;
using NUnit.Framework;

namespace DigitalFailState.Tests
{
    [TestFixture]
    public class ServiceTests
    {
        [Test]
        public void QuestionFactory() {
            IQuestionProvider qf = new QuestionFactory();
            var next = qf.GetNextQuestion();
            var byId = qf.GetQuestionById(next.Id);
            Assert.AreEqual(next.Question, byId.Question);
        }

        [Test]
        public void ScoreProvider() {
            var ss = new StorageService();
            var sp = new ScoreProvider(ss);
            var next = sp.GetNextScore();
            Assert.AreEqual(next, sp.GetScore());
        }
    }
}
