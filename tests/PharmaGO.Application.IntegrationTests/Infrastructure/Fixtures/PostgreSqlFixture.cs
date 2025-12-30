using Microsoft.EntityFrameworkCore;
using PharmaGO.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;

public class PostgreSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:18-alpine")
            .WithDatabase("pharmago_test")
            .WithUsername("test")
            .WithPassword("test")
            .WithCleanUp(true)
            .Build();

        await Container.StartAsync();

        // Aplicar migrations
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }

    public PharmaGOContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PharmaGOContext>()
            .UseNpgsql(Container.GetConnectionString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        return new PharmaGOContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}