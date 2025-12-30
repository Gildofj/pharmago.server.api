using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmaGO.Application.Common.Auth.Constants;
using PharmaGO.Application.IntegrationTests.Infrastructure.Factories;
using PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;
using PharmaGO.Application.IntegrationTests.Infrastructure.Identity;
using PharmaGO.Application.IntegrationTests.Infrastructure.Seeds;
using PharmaGO.Application.IntegrationTests.Infrastructure.Utils;
using PharmaGO.Contract.Authentication;
using PharmaGO.Core.Common.Constants;
using PharmaGO.Core.Entities;
using PharmaGO.Infrastructure.Persistence;

namespace PharmaGO.Application.IntegrationTests.Infrastructure;

public class IntegrationTestBase
    : IClassFixture<PostgreSqlFixture>, IClassFixture<EnviromentVarsFixture>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory Factory;
    protected readonly IConfiguration Configuration;
    protected readonly PostgreSqlFixture DbFixture;
    protected readonly HttpClient HttpClient;
    protected PharmaGOContext Context => DbFixture.CreateContext();
    protected Pharmacy TestPharmacy;

    protected IntegrationTestBase(PostgreSqlFixture dbFixture, EnviromentVarsFixture envVarsFixture)
    {
        Configuration = envVarsFixture.Configuration;
        DbFixture = dbFixture;

        Factory = new CustomWebApplicationFactory(DbFixture.Container.GetConnectionString());

        HttpClient = Factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {
        await DbFixture.ResetDatabaseAsync();

        await using var context = Context;
        var seeder = new TestDataSeeder(context, Configuration);
        TestPharmacy = await seeder.SeedAsync();
    }

    public virtual Task DisposeAsync()
    {
        HttpClient.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<Guid> CreateTestPharmacyAsync()
    {
        await using var context = Context;
        var pharmacy = TestDataGenerator.CreatePharmacy();
        await context.Pharmacies.AddAsync(pharmacy);
        await context.SaveChangesAsync();
        return pharmacy.Id;
    }

    protected HttpClient GetAuthorizedClient()
    {
        var client = Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationSchemeProvider.Name);

        client.DefaultRequestHeaders.Add("role", nameof(UserType.Client));
        
        List<string> permissions = [Permissions.ClientAccess]; 
        
        client.DefaultRequestHeaders.Add("permissions", permissions);

        return client;
    }

    protected HttpClient GetAuthorizedEmployee()
    {
        var client = Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationSchemeProvider.Name);

        client.DefaultRequestHeaders.Add("role", EmployeeRoles.Employee);

        List<string> permissions = [Permissions.ManageProducts]; 
        
        client.DefaultRequestHeaders.Add("permissions", permissions);

        return client;
    }

    protected HttpClient GetAuthorizedAdmin()
    {
        var client = Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationSchemeProvider.Name);

        client.DefaultRequestHeaders.Add("role", EmployeeRoles.Admin);
        
        List<string> permissions = [Permissions.ManageProducts]; 
        
        client.DefaultRequestHeaders.Add("permissions", permissions);

        return client;
    }

    protected HttpClient GetAuthorizedMasterAdmin()
    {
        var client = Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthenticationSchemeProvider.Name);

        client.DefaultRequestHeaders.Add("role", nameof(UserType.MasterAdmin));
        
        List<string> permissions = Permissions.All.ToList(); 
        
        client.DefaultRequestHeaders.Add("permissions", permissions);

        return client;
    }
}