using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tmds.Systemd;

namespace DigitalFailState.Web.HostedServices
{
    public class WatchDog : IHostedService, IDisposable {
        private readonly ILogger<WatchDog> _logger;

        public WatchDog(ILogger<WatchDog> logger) {
            _logger = logger;
        }

        private Timer _timer = null;

        public Task StartAsync(CancellationToken cancellationToken) {
            var systemD = ServiceManager.Notify(ServiceState.Ready);
            if (systemD) {
                _logger.LogInformation("signaling ready to systemD");
            }
            else {
                _logger.LogWarning("systemD=false");
            }
            var watchdogUsecStr = Environment.GetEnvironmentVariable("WATCHDOG_USEC");
            if (!string.IsNullOrWhiteSpace(watchdogUsecStr) && long.TryParse(watchdogUsecStr, out var watchdogUsec)) {
                var watchdogMsec = watchdogUsec / 1000L - 10L;
                _timer = new Timer(PingSystemD, null, 0, watchdogMsec);
            }
            else {
                _logger.LogWarning($"WATCHDOG_USEC={watchdogUsecStr}");
            }
            return Task.CompletedTask;
        }

        private void PingSystemD(object state) {
            ServiceManager.Notify(ServiceState.Watchdog);
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            ServiceManager.Notify(ServiceState.Stopping);
            return Task.CompletedTask;
        }

        public void Dispose() {
            _timer?.Dispose();
        }
    }
}
