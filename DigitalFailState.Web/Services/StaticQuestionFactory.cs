using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DigitalFailState.Web.Models;

namespace DigitalFailState.Web.Services
{
    public class StaticQuestionFactory : IQuestionProvider
    {
        private static readonly object Sync = new object();
        private static int _questionId = 0;
        private static readonly ImmutableList<QuestionModel> Questions = StaticQuestionFactory.Init();

        private static ImmutableList<QuestionModel> Init() {
            var cfg = new Configuration() {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = false,
                Delimiter = ",",
                Quote = '"',
            };

            var qm = new List<QuestionModel>();
            var id = 1;

            using (var fs = File.OpenRead("questions.csv")) 
            using (var sr = new StreamReader(fs,cfg.Encoding)) {
                var cr = new CsvReader(sr, cfg);
                while (cr.Read()) {
                    qm.Add(new QuestionModel {
                        Id = id++,
                        Question = cr.GetField<string>(0),
                        YesConclusion = cr.GetField<string>(1),
                        NoConclusion = cr.GetField<string>(2),
                    });
                }
            }

            return qm.ToImmutableList();
        }

        public QuestionModel GetNextQuestion() {
            lock (Sync) {
                var lastId = Questions.Max(q => q.Id);
                _questionId++;
                if (_questionId > lastId) _questionId = 1;
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
