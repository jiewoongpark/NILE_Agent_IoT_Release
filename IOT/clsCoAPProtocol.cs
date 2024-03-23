using CoAP;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IOT
{
    
    public class ClsCoApProtocol
    {
        private static CoapClient _client;

        public delegate void MessageReceivedHandler(string resourcePath, string message);
        public event MessageReceivedHandler MessageReceived;

        public ClsCoApProtocol(string url)
        {
            _client = new CoapClient(new Uri(url));
        }

        public async Task RetrieveMessageAsync(string topic)
        {
            var responseTask = Task.Run(() => _client.Get());
            var response = await responseTask;
            var message = Encoding.UTF8.GetString(response.Payload);
            MessageReceived?.Invoke(topic, message);
        }
    }

}

