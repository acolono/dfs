using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace DigitalFailState.Web.Services
{
    public class MqttService
    {
        private readonly ILogger<MqttService> _logger;

        public MqttService(ILogger<MqttService> logger) {
            _logger = logger;
        }

        public async Task UpdateScoreSilentAsync(long score) {
            try {
                await UpdateScoreAsync(score);
            }
            catch (Exception e) {
                _logger.LogError(e,"mqtt error");
            }
        }

        private async Task UpdateScoreAsync(long score) {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString().Replace("-",""))
                .WithTcpServer("iot.eclipse.org")
                .WithTls()
                .WithCleanSession()
                .Build();
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            await client.ConnectAsync(options);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("aes-2018/digital-fail-state/score")
                .WithPayload(score.ToString())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            await client.PublishAsync(message);
        }
    }
}
