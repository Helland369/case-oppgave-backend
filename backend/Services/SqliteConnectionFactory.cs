using Microsoft.Data.Sqlite;

namespace Backend.Services;

public class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly string _connection;

    public SqliteConnectionFactory(string connection)
    {
        _connection = connection;
    }

    public SqliteConnection Create()
    {
        var conn = new SqliteConnection(_connection);
        conn.Open();
        return conn;
    }
}
