using MySqlConnector;
using Speck.DurableMessaging.Common;

namespace Speck.DurableMessaging.MySql;

internal class MySqlUnitOfWork(MySqlConnection connection) : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();

        await action();
        
        await transaction.CommitAsync();
    }
}