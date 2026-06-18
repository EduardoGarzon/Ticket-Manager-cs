using System.Data;
using MySqlConnector;

namespace TicketManager.Infrastructure;

public class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CriarConexao()
    {
        return new MySqlConnection(_connectionString);
    }
}