using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PharmaGO.Application.Common.Auth.Constants;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Identity;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock
    )
    {
    }

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>();

        if (Context.Request.Headers.TryGetValue("role", out var roles))
        {
            claims.Add(new Claim(ClaimTypes.Role, roles[0]!));
        }

        if (Context.Request.Headers.TryGetValue("permissions", out var permissions))
        {
            claims.AddRange(permissions.Select(p => new Claim(CustomClaims.Permission, p!)));
        }

        var identity = new ClaimsIdentity(claims, TestAuthenticationSchemeProvider.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthenticationSchemeProvider.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}