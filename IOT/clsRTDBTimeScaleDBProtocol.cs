using Npgsql;
using System;
using System.Threading.Tasks;

namespace IOT
{
    public class ClsRtdbTimeScaleDbProtocol
    {
        private static string _connectionString;

        public ClsRtdbTimeScaleDbProtocol(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task WriteDataAsync(string tableName, string topic, string message)
        {
            var query = $"INSERT INTO {tableName} (topic, message, timestamp) VALUES (@topic, @message, @timestamp)";
        
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@topic", topic);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
    
}


