using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeRedis;
using StackExchange.Redis;
using System.Threading.Tasks;



namespace IOT
{
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class ClsRtdbRedisProtocol
    {

        private IDatabase _database;
        private ConnectionMultiplexer _connectionMultiplexer;
        private int targetDatabase { get; }
        // public string connectedAt { get; private set; }
        public bool connectionStatus { get; private set; }
        // public string listLengthView { get; private set; }
        // public string connectionEx { get; private set; }
        
        // Clustered Redis Only
        private RedisClient _redisClient;
        private string realConnection;
        
        public ClsRtdbRedisProtocol(string connectionString, int database, bool clustered, string finalTopic)
        {

            if (clustered)
            {
                connectionStatus = false;
                
                string[] connectionStrings = connectionString.Split(';');
                
                foreach (string connStr in connectionStrings) 
                {
                    try
                    {
                        _redisClient = new RedisClient(connStr);

                        _redisClient.LLen(finalTopic);

                        realConnection = connStr;
                        
                        connectionStatus = true;

                        break;
                    }
                    catch (Exception ex) when (ex.Message.Contains("MOVED"))
                    {
                       
                    }
                }
                
                /*string[] connectionStringParts = connectionString.Split(';');
                
                ConnectionStringBuilder[] clusterConnectionStrings = connectionStringParts.Select(connStr =>
                        (ConnectionStringBuilder)connStr
                ).ToArray();
                
                Dictionary<string, string> hostMappings = null;


                _redisClient = new RedisClient(clusterConnectionStrings, hostMappings);

                connectionStatus = true;*/
                
            }
            else
            {
                
                targetDatabase = database;
                
                connectionStatus = false;
                _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
                connectionStatus = true;
                
            }
            
        }

        
        public async Task AppendDataToListAsync(string key, string value, bool MessageTime, bool clustered, string connectionString)
        {

            if (clustered)
            {
                
                string messageValue;
            
                if (MessageTime)
                {
                    var utcNow = DateTime.UtcNow;
                    var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                    var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
                    var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

                    messageValue = $"[{timestamp}] {value}";
                }
                else
                {
                    messageValue = $"{value}";
                }
                
                var redisClient = new RedisClient(realConnection);
                        
                await redisClient.RPushAsync(key, messageValue);
                
                /*string[] connectionStrings = connectionString.Split(';');
                
                foreach (string connStr in connectionStrings) 
                {
                    try
                    {
                        var redisClient = new RedisClient(connStr);
                        
                        await redisClient.RPushAsync(key, messageValue);
                        
                        break;
                    }
                    catch (Exception ex) when (ex.Message.Contains("Connect to redis-server"))
                    {
                       
                    }
                }*/
                
            }
            else
            {
                _database = _connectionMultiplexer.GetDatabase(targetDatabase);

                string messageValue;
            
                if (MessageTime)
                {
                    var utcNow = DateTime.UtcNow;
                    var koreanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                    var koreanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanTimeZone);
                    var timestamp = koreanTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

                    messageValue = $"[{timestamp}] {value}";
                }
                else
                {
                    messageValue = $"{value}";
                }
            
                await _database.ListRightPushAsync(key, messageValue);
            }
            

        }
        
    }
    
    
}