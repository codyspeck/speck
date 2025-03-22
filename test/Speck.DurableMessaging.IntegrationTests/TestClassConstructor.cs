using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TUnit.Core.Interfaces;

namespace Speck.DurableMessaging.IntegrationTests;

public class TestClassConstructor : IClassConstructor, ITestEndEventReceiver
{
    private static readonly IServiceProvider Services;

    private AsyncServiceScope _scope;

    static TestClassConstructor()
    {
        Services = new ServiceCollection()
            .AddSingleton<MySqlFixture>()
            .AddSingleton<Fixture>()
            .AddScoped<MySqlConnection>(services => services
                .GetRequiredService<MySqlFixture>()
                .CreateConnection())
            .BuildServiceProvider();
    }

    [Before(TestSession)]
    public static async Task BeforeTestSession()
    {
        await Services
            .GetRequiredService<MySqlFixture>()
            .InitializeAsync();
    }
    
    public T Create<T>(ClassConstructorMetadata classConstructorMetadata) where T : class
    {
        _scope = Services.CreateAsyncScope();
        
        return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
    }

    public ValueTask OnTestEnd(TestContext testContext)
    { 
        return _scope.DisposeAsync();
    }
}