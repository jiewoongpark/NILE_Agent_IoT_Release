using StackExchange.Redis;
using System;
using System.Globalization;
using System.Threading.Tasks;


namespace IOT
{
    public class ClsRtdbRedisStandaloneProtocol
    {
        
        private static IDatabase _database;
        private static ConnectionMultiplexer _connectionMultiplexer;

        
        public ClsRtdbRedisStandaloneProtocol(string connectionString, int database)
        {
            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
            {
                _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            }
            _database = _connectionMultiplexer.GetDatabase(database);
        }

        
        public async Task AppendDataToListAsync(string key, string value)
        {
            var utcNow = DateTime.UtcNow;
            var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
            var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

            var valueWithTimestamp = $"[{timestamp}] {value}";

            await _database.ListRightPushAsync(key, valueWithTimestamp);
        }
        
    }
    
}