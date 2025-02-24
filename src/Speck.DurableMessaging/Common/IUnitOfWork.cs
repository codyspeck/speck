namespace Speck.DurableMessaging.Common;

internal interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(Func<Task> action);
}