using System;
using System.Threading;

namespace DigitalFailState.Web.Services {
    public class ScoreProvider {
        private readonly IStorageService _storageService;
        private static long _score = 0;

        public ScoreProvider(IStorageService storageService) {
            _storageService = storageService;
            if (_score == 0) {
                _score = _storageService.LoadScore();
            }
        }

        public long GetNextScore() {
            var lost = new Random().Next(12,21);
            var next = Interlocked.Add(ref _score, 0 - lost);
            _storageService.SaveStore(next);
            return next;
        }

        public long GetScore() => Interlocked.Read(ref _score);
    }
}