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
        
        /*public async Task WriteDataBatchAsync(IEnumerable<(string topic, string message)> data, string tableName)
        {
            var query = $"INSERT INTO {tableName} (topic, message, timestamp) VALUES (@topic, @message, @timestamp)";
            
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction())
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@topic", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters.Add(new NpgsqlParameter("@message", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters.Add(new NpgsqlParameter("@timestamp", NpgsqlTypes.NpgsqlDbType.Timestamp));

                    foreach (var (topic, message) in data)
                    {
                        cmd.Parameters["@topic"].Value = topic;
                        cmd.Parameters["@message"].Value = message;
                        cmd.Parameters["@timestamp"].Value = DateTime.UtcNow;
                        await cmd.ExecuteNonQueryAsync();
                    }

                    await trans.CommitAsync();
                }
            }
        }*/
    }
    
}


