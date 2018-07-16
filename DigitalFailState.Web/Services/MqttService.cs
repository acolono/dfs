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
        private readonly IMqttClientOptions _options;

        public MqttService(ILogger<MqttService> logger) {
            _logger = logger;
            _options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString().Replace("-",""))
                .WithTcpServer("iot.eclipse.org")
                .WithTls()
                .WithCleanSession()
                .Build();
        }

        public async Task UpdateScoreSilentAsync(long score) {
            try {
                await UpdateScoreAsync(score);
            }
            catch (Exception e) {
                _logger.LogError(e, "mqtt error");
            }
        }

        private async Task UpdateScoreAsync(long score) {
            var factory = new MqttFactory();
            using (var client = factory.CreateMqttClient()) {
                await client.ConnectAsync(_options);
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("aes-2018/digital-fail-state/score")
                    .WithPayload(score.ToString())
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();
                await client.PublishAsync(message);
                await client.DisconnectAsync();    
            }
        }
    }
}
