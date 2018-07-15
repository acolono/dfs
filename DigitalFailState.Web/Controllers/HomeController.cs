using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalFailState.Web.Models;
using DigitalFailState.Web.Services;

namespace DigitalFailState.Web.Controllers
{
    public class HomeController : Controller {
        private readonly ScoreProvider _scoreProvider;

        public HomeController(ScoreProvider scoreProvider) {
            _scoreProvider = scoreProvider;
        }

        public IActionResult Index() => View();

        public IActionResult App() => View();

        public IActionResult Score() => View();

        public IActionResult LegacyScore() {
            ViewBag.Score = _scoreProvider.GetScore();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
