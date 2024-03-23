using FreeRedis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace IOT
{
    
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
    [SuppressMessage("ReSharper", "EmptyConstructor")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class ClsRtdbRedisProtocol
    {
        
        public ClsRtdbRedisProtocol()
        {
        }
        
        
        private static RedisClient _redisClient;
        
        private static bool _isInitialized = false;

        private string _keyPrefix = "";
        

        public static OperationResult Initialize(string connectionString, int database)
        {
            try
            {
                if (_redisClient == null)
                {
                    _redisClient = new RedisClient(connectionString);
                    /*
                    string keyPrefix = $"db{database}:";
                    */
                    var keyPrefix = $"db{database}:";

                    
                    _isInitialized = true;
                }
                return new OperationResult(true, "Connection Established Successfully to REDIS");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, $"Exception while attempting to connect to REDIS: {ex.Message}");
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

                /*string valueWithTimestamp = $"[{timestamp}] {value}";*/
                var valueWithTimestamp = $"[{timestamp}] {value}";

                await _redisClient.RPushAsync(_keyPrefix + key, valueWithTimestamp);
                
                return new OperationResult(true, "Data inserted successfully to REDIS.");

            }
            catch (Exception ex)
            {
                return new OperationResult(false, $"Error inserting data into REDIS: {ex.Message}");
            }

        }
        
        
        
    
    
        
        /*In case where multiple inserting action to each Redis node is required*/
        
        private static Dictionary<string, RedisClient> _redisClients = new Dictionary<string, RedisClient>();
        
        private static bool _isInitializedManyNodes = false;

        private string _keyPrefixManyNodes = "";
        
        
        
        public static OperationResult InitializeManyNodes(IEnumerable<string> connectionStrings, int database)
        {
            try
            {
                foreach (var connectionString in connectionStrings)
                {
                    if (!_redisClients.ContainsKey(connectionString))
                    {
                        var client = new RedisClient(connectionString);
                        _redisClients.Add(connectionString, client);
                    }
                }
                _isInitializedManyNodes = true;
                return new OperationResult(true, "Connections established successfully to REDIS nodes.");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, $"Exception while attempting to connect to REDIS: {ex.Message}");
            }
        }

        public static bool RedisIsInitializedManyNodes => _isInitializedManyNodes;

        public async Task<OperationResult> AppendDataToManyNodesListAsync(string key, string value)
        {
            if (!_isInitializedManyNodes)
            {
                return new OperationResult(false, "Redis clients are not initialized.");
            }

            var utcNow = DateTime.UtcNow;
            var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
            var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

            var valueWithTimestamp = $"[{timestamp}] {value}";
            
            var exceptions = new List<Exception>();

            foreach (var client in _redisClients.Values)
            {
                try
                {
                    await client.RPushAsync(_keyPrefixManyNodes + key, valueWithTimestamp);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                return new OperationResult(false, $"Error inserting data into one or more REDIS nodes: {exceptions[0].Message}");
            }

            return new OperationResult(true, "Data inserted successfully to all specified REDIS nodes.");
        }
        
    }

    

    
    
    

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

