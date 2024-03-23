using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading.Tasks;

namespace IOT
{
    
    public class clsMQTTBrokerProtocol
    {
        private readonly IMqttClient _client;
        private readonly IMqttClientOptions _options;
        public event Action<string, string> MessageReceived;

        public clsMQTTBrokerProtocol(string url, int port, string clientId, string username, string password, bool cleanSession, int keepAlivePeriod)
        {
            var mqttFactory = new MqttFactory();
            _client = mqttFactory.CreateMqttClient();

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(url, port)
                .WithCleanSession(cleanSession)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(keepAlivePeriod));

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                builder.WithCredentials(username, password);
            }

            _options = builder.Build();

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                
                MessageReceived?.Invoke($"{topic}", payload);
            });
        }


        public async Task ConnectAndSubscribeAsync(string topic)
        {
            await _client.ConnectAsync(_options);
            await _client.SubscribeAsync(topic);
        }

        public async Task DisconnectAndUnsubscribeAsync()
        {
            await _client.DisconnectAsync();
        }

        // Method to check if the MQTT client is connected
        public bool IsConnected()
        {
            return _client?.IsConnected ?? false;
        }
        
    }

}
