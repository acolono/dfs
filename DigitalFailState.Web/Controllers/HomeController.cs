using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DigitalFailState.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using DigitalFailState.Web.Models;
using DigitalFailState.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DigitalFailState.Web.Controllers
{
    public class HomeController : Controller {
        private readonly ScoreProvider _scoreProvider;

        public HomeController(ScoreProvider scoreProvider) {
            _scoreProvider = scoreProvider;
        }

        [HttpGet("/")]
        public IActionResult Index() {
            lock (AppHub.ActiveQuestions) {
                var t = AppHub.ActiveQuestions.Select(q => new {connectionId = q.Key, questionId = q.Value});
                ViewBag.questions = JToken.FromObject(t).ToString(Formatting.Indented);
            }
            return View();
        }

        [HttpGet("/app")]
        public IActionResult App() => View();

        [HttpGet("/score")]
        public IActionResult Score() => View();

        [HttpGet("/score/legacy")]
        public IActionResult LegacyScore() {
            ViewBag.Score = _scoreProvider.GetScore();
            return View();
        }

        [HttpGet("/score/mqtt")]
        public IActionResult MqttScore() => View();
    }
}
