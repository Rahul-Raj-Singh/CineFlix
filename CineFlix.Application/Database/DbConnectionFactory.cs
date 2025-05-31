using System.Data;
using Npgsql;

namespace CineFlix.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> GetConnection();
}

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> GetConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}