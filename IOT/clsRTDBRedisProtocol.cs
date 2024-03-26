using FreeRedis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;

namespace IOT
{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    [SuppressMessage("ReSharper", "EmptyConstructor")]
    public class ClsRtdbRedisProtocol
    {
        private static RedisClient _redisClient;
        private static bool _isInitialized = false;
        private string _keyPrefix = "";

        public ClsRtdbRedisProtocol() { }

        public static OperationResult Initialize(string nodeEndpoints)
        {
            try
            {
                if (_redisClient == null)
                {
                    var connectionString = string.Join(",", nodeEndpoints);
                    _redisClient = new RedisClient(connectionString);

                    _isInitialized = true;
                }
                return new OperationResult(true, "Connection Established Successfully to REDIS Cluster");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, $"Exception while attempting to connect to REDIS Cluster: {ex.Message}");
            }
        }

        public static bool RedisIsInitialized => _isInitialized;

        public async Task<OperationResult> AppendDataToListAsync(string key, string value)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
                var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

                var valueWithTimestamp = $"[{timestamp}] {value}";

                await _redisClient.RPushAsync(_keyPrefix + key, valueWithTimestamp);

                return new OperationResult(true, "Data inserted successfully to REDIS.");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, $"Error inserting data into REDIS: {ex.Message}");
            }
        }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class OperationResult
    {
        public bool RedisSuccess { get; set; }
        public string RedisMessage { get; set; }

        public OperationResult(bool success, string message)
        {
            RedisSuccess = success;
            RedisMessage = message;
        }
    }
}
