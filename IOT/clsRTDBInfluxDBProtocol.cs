using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;

namespace IOT
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class clsRTDBInfluxDBProtocol
    {
        private readonly InfluxDBClient _client;
        private readonly string _bucket;
        private readonly string _org;

        public static bool influxConnectionStatus { get; private set; }
        public static string influxConnectionMessage { get; private set; }


        public clsRTDBInfluxDBProtocol(string url, string token, string org, string bucket)
        {
            try
            {
                influxConnectionStatus = false;
                
                var options = new InfluxDBClientOptions.Builder()
                    .Url(url)
                    .AuthenticateToken(token.ToCharArray())
                    .Build();

                _client = new InfluxDBClient(options);
                _bucket = bucket;
                _org = org;

                influxConnectionStatus = true;
            }
            catch (Exception ex)
            {
                influxConnectionMessage = ex.Message;
                influxConnectionStatus = false;
            }
            
        }
        
        
        public async Task WriteData(string measurement, string topic, string message, bool MessageTime)
        {
            var writeApi = _client.GetWriteApiAsync();

            string messageValue;
            
            if (MessageTime)
            {
                var utcNow = DateTime.UtcNow;
                var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
                var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

                messageValue = $"[{timestamp}] {message}";
            }
            else
            {
                messageValue = message;
            }
            
            var point = PointData.Measurement(measurement)
                .Tag("topic", topic)
                .Field("message", messageValue)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            await writeApi.WritePointAsync(point, _bucket, _org);
            
        }
        
        
        
    }
}
