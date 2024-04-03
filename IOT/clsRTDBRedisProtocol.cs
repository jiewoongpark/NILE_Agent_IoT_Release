using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using StackExchange.Redis;
using System.Threading.Tasks;
#pragma warning disable CS0184 // 'is' expression's given expression is never of the provided type
#pragma warning disable CS0183 // 'is' expression's given expression is always of the provided type


namespace IOT
{
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public class ClsRtdbRedisProtocol
    {
        
        private static IDatabase _database;
        private static ConnectionMultiplexer _connectionMultiplexer;
        private static int targetDatabase;

        // public long connectedAt { get; private set; }
        public bool connectionStatus { get; private set; }

        
        
        public ClsRtdbRedisProtocol(string connectionString, int database, bool clustered, string finalizedTopic)
        {
            targetDatabase = database;

            if (clustered)
            {
                if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
                {
                    connectionStatus = false;
                    
                    string[] connectionStrings = connectionString.Split(';');
                    
                    foreach (string connStr in connectionStrings)
                    {
                   
                        var multiplexer = ConnectionMultiplexer.Connect(connStr);
                        var listLength = multiplexer.GetDatabase(targetDatabase).ListLength(finalizedTopic);
                        
                        if (listLength is long || listLength is int)
                        {
                            _connectionMultiplexer = multiplexer;
                            connectionStatus = true;
                            break;
                        }
                    
                    }
                }
                else
                {
                    connectionStatus = false;
                }
            }
            else
            {
                connectionStatus = false;
                _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
                connectionStatus = true;
            }
            
        }

        
        public async Task AppendDataToListAsync(string key, string value, bool MessageTime)
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