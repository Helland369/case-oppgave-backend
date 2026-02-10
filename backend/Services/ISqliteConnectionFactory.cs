using Microsoft.Data.Sqlite;

namespace Backend.Services;

public interface ISqliteConnectionFactory
{
    SqliteConnection Create();
}
