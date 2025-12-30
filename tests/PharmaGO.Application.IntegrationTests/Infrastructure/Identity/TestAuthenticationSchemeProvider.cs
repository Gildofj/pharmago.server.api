using Bogus.DataSets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Identity;

public class TestAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    public const string Name = "TestAuthenticationScheme";
    
    public TestAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    protected TestAuthenticationSchemeProvider(
        IOptions<AuthenticationOptions> options,
        IDictionary<string, AuthenticationScheme> schemes
    ) : base(options, schemes)
    {
    }

    public override Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync()
    {
        var scheme = new AuthenticationScheme(Name, Name, typeof(TestAuthenticationHandler));
        return Task.FromResult(scheme)!;
    }
}