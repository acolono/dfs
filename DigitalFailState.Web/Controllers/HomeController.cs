using Microsoft.AspNetCore.Mvc;
using DigitalFailState.Web.Services;
using Microsoft.Extensions.FileProviders;

namespace DigitalFailState.Web.Controllers
{
    public class HomeController : Controller {
        private readonly ScoreProvider _scoreProvider;

        public HomeController(ScoreProvider scoreProvider) {
            _scoreProvider = scoreProvider;
        }

        [HttpGet("/")]
        public IActionResult Index() => View();

        [HttpGet("/app")]
        public IActionResult App() => View();

        [HttpGet("/score")]
        public IActionResult Score() => View();

        [HttpGet("/score/legacy")]
        public IActionResult LegacyScore() {
            ViewBag.Score = _scoreProvider.GetScore();
            return View();
        }

        [HttpGet("/android-app")]
        public IActionResult AndroidApp() => GetFile("dfs.apk");

        [HttpGet("/debug/android-app")]
        public IActionResult AndroidDebugApp() => GetFile("dfs-debug.apk");

        private IActionResult GetFile(string fileName, string downloadName = null, string contentType = null) {
            var fp = new ManifestEmbeddedFileProvider(typeof(StaticQuestionFactory).Assembly);
            var fi = fp.GetFileInfo(fileName);
            var stream = fi.CreateReadStream();
            return File(stream, contentType ?? "application/vnd.android.package-archive", downloadName ?? fileName);
        }
    }
}
