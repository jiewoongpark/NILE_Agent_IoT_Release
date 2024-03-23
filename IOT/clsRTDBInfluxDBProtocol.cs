using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Threading.Tasks;

namespace IOT
{
    public class clsRTDBInfluxDBProtocol
    {
        private readonly InfluxDBClient _client;
        private readonly string _bucket;
        private readonly string _org;

        public clsRTDBInfluxDBProtocol(string url, string token, string org, string bucket)
        {
            var options = new InfluxDBClientOptions.Builder()
                .Url(url)
                .AuthenticateToken(token.ToCharArray())
                .Build();

            _client = new InfluxDBClient(options);
            _bucket = bucket;
            _org = org;
        }
        
        
        public async Task WriteData(string measurement, string topic, string message)
        {
            var writeApi = _client.GetWriteApiAsync();

            var point = PointData.Measurement(measurement)
                .Tag("topic", topic)
                .Field("message", message)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            await writeApi.WritePointAsync(point, _bucket, _org);
            
        }
        
        
        
    }
}
