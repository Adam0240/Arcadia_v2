#nullable enable

using Microsoft.Data.Sqlite;

namespace Arcadia_v2.Saves
{
    public sealed class SqliteGameSaveRepository : IGameSaveRepository
    {
        private const int SlotId = 1;
        private readonly string mDatabasePath;

        public SqliteGameSaveRepository()
            : this(Path.Combine(AppContext.BaseDirectory, "data", "savegame.db"))
        {
        }

        public SqliteGameSaveRepository(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("Database path cannot be empty.", nameof(databasePath));
            }

            mDatabasePath = databasePath;
        }

        public void Initialize()
        {
            string? dataDirectory = Path.GetDirectoryName(mDatabasePath);

            if (!string.IsNullOrWhiteSpace(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            using SqliteConnection connection = OpenConnection();
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS saves (
                    slot_id INTEGER PRIMARY KEY,
                    save_json TEXT NOT NULL,
                    updated_utc TEXT NOT NULL
                );
                """;

            command.ExecuteNonQuery();
        }

        public void SaveJson(string saveJson)
        {
            if (string.IsNullOrWhiteSpace(saveJson))
            {
                throw new ArgumentException("Save JSON cannot be empty.", nameof(saveJson));
            }

            using SqliteConnection connection = OpenConnection();
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText =
                """
                INSERT INTO saves (slot_id, save_json, updated_utc)
                VALUES ($slot_id, $save_json, $updated_utc)
                ON CONFLICT(slot_id) DO UPDATE SET
                    save_json = excluded.save_json,
                    updated_utc = excluded.updated_utc;
                """;

            command.Parameters.AddWithValue("$slot_id", SlotId);
            command.Parameters.AddWithValue("$save_json", saveJson);
            command.Parameters.AddWithValue("$updated_utc", DateTimeOffset.UtcNow.ToString("O"));

            command.ExecuteNonQuery();
        }

        public string? LoadJson()
        {
            using SqliteConnection connection = OpenConnection();
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText =
                """
                SELECT save_json
                FROM saves
                WHERE slot_id = $slot_id;
                """;

            command.Parameters.AddWithValue("$slot_id", SlotId);

            object? result = command.ExecuteScalar();
            return result as string;
        }

        public bool DeleteSave()
        {
            using SqliteConnection connection = OpenConnection();
            using SqliteCommand command = connection.CreateCommand();

            command.CommandText =
                """
                DELETE FROM saves
                WHERE slot_id = $slot_id;
                """;

            command.Parameters.AddWithValue("$slot_id", SlotId);

            return command.ExecuteNonQuery() > 0;
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnectionStringBuilder builder = new()
            {
                DataSource = mDatabasePath,
                Pooling = false
            };

            SqliteConnection connection = new(builder.ToString());
            connection.Open();
            return connection;
        }
    }
}
