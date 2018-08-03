using System;
using System.Threading;
using System.Threading.Tasks;
using DigitalFailState.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Controllers {
    public class ApiController : ControllerBase {
        private readonly IHubContext<AppHub> _appHubContext;
        private IActionResult GoHome => RedirectToAction("Index", "Home");

        public ApiController(IHubContext<AppHub> appHubContext) {
            _appHubContext = appHubContext;
        }

        [HttpGet("/restart")]
        public async Task<IActionResult> Restart() {
            await _appHubContext.Clients.All.SendAsync("Restart");
            return GoHome;
        }

        [HttpGet("/crash")]
        public IActionResult Crash() {
            var doomTimer = new Timer(state => { Environment.FailFast("requested by user"); }, null, 1000, 1000);
            return GoHome;
        }
    }
}