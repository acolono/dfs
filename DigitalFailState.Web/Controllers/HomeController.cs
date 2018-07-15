using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalFailState.Web.Models;

namespace DigitalFailState.Web.Controllers
{
    public class HomeController : Controller {
        public IActionResult Index() => View();

        public IActionResult App() => View();

        public IActionResult Score() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
