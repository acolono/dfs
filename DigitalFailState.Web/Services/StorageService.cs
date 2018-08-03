using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DigitalFailState.Web.Services
{
    public interface IStorageService {
        void SaveStore(long score);
        long LoadScore();
    }

    public class StorageService : IStorageService {

        private static readonly object Sync = new object();
        private const string ScoreFile = "score.txt";

        public void SaveStore(long score) {
            lock (Sync) {
                File.WriteAllText(ScoreFile, score.ToString(), Encoding.UTF8);
            }
        }

        public long LoadScore() {
            string content;
            lock (Sync) {
                if (!File.Exists(ScoreFile)) return 0;
                content = File.ReadAllText(ScoreFile, Encoding.UTF8);
            }
            return long.TryParse(content, out var score) ? score : 0;
        }
    }
}
