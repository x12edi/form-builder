using Microsoft.Data.Sqlite;
using System.IO;

namespace form_builder.Data
{
    public static class DatabaseHelper
    {
        private static readonly string _dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "forms.db");
        private static readonly string _connStr = $"Data Source={_dbPath}";

        public static string ConnectionString => _connStr;

        public static void Initialize()
        {
            if (!File.Exists(_dbPath))
            {
                //SqliteConnection.CreateFile(_dbPath);
            }

            using var conn = new SqliteConnection(_connStr);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = File.ReadAllText("Data/init.sql");
            command.ExecuteNonQuery();
        }
    }
}
