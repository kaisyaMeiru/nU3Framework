using Microsoft.Data.Sqlite;

Console.WriteLine("SQLite test starting...");
var connStr = "Data Source=./test_db.sqlite;";
using var conn = new SqliteConnection(connStr);
try
{
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "CREATE TABLE IF NOT EXISTS test(id INTEGER PRIMARY KEY, name TEXT);";
    cmd.ExecuteNonQuery();

    cmd.CommandText = "INSERT INTO test(name) VALUES($name);";
    cmd.Parameters.AddWithValue("$name", "hello");
    cmd.ExecuteNonQuery();

    cmd.CommandText = "SELECT id, name FROM test;";
    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"row: {reader.GetInt32(0)} / {reader.GetString(1)}");
    }

    Console.WriteLine("SQLite test succeeded.");
}
catch (Exception ex)
{
    Console.WriteLine("SQLite test failed: " + ex.Message);
}
finally
{
    conn.Close();
}