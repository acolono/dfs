using System;
using System.Threading;

namespace DigitalFailState.Web.Services {
    public class ScoreProvider {
        private static long _score = 0;

        public long GetNextScore() {
            var lost = new Random().Next(12,21);
            return Interlocked.Add(ref _score, 0 - lost);
        }

        public long GetScore() => Interlocked.Read(ref _score);
    }
}