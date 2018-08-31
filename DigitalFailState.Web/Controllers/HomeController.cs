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
        public IActionResult AndroidApp() {
            var fp = new ManifestEmbeddedFileProvider(typeof(StaticQuestionFactory).Assembly);
            var fi = fp.GetFileInfo("dfs.apk");
            var stream = fi.CreateReadStream();
            return File(stream, "application/vnd.android.package-archive", "dfs.apk");
        }
    }
}
