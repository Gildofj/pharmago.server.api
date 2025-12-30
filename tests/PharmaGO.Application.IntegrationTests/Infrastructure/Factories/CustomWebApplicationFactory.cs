using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PharmaGO.Application.IntegrationTests.Infrastructure.Identity;
using PharmaGO.Infrastructure.Persistence;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Factories;

public class CustomWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PharmaGOContext>>();
            services.RemoveAll<PharmaGOContext>();

            services.AddDbContext<PharmaGOContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PharmaGOContext>();

            context.Database.EnsureCreated();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();

            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationSchemeProvider.Name;
                        options.DefaultChallengeScheme = TestAuthenticationSchemeProvider.Name;
                    }
                )
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationSchemeProvider.Name, _ => { }
                );
        });

        builder.UseEnvironment("Test");
    }
}