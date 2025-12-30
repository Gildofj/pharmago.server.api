using Microsoft.Extensions.Configuration;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;

public class EnviromentVarsFixture
{
    public IConfiguration Configuration { get; }

    public EnviromentVarsFixture()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Test.json", false)
            .AddEnvironmentVariables()
            .Build();
    }
}