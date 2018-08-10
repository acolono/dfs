using System.IO;
using System.Text;

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
            try {
                lock (Sync) {
                    File.WriteAllText(ScoreFile, score.ToString(), Encoding.UTF8);
                }
            }
            catch (IOException) {
                // my bad...
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
