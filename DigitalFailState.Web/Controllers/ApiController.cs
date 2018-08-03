using System.Threading.Tasks;
using DigitalFailState.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DigitalFailState.Web.Controllers {
    public class ApiController : ControllerBase {
        private readonly IHubContext<AppHub> _appHubContext;

        public ApiController(IHubContext<AppHub> appHubContext) {
            _appHubContext = appHubContext;
        }

        [HttpGet("/restart")]
        public async Task<IActionResult> Restart() {
            await _appHubContext.Clients.All.SendAsync("Restart");
            return RedirectToAction("Index", "Home");
        }
    }
}