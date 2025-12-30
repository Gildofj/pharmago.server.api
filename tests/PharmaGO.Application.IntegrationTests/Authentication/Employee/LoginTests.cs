using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PharmaGO.Application.IntegrationTests.Infrastructure;
using PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;
using PharmaGO.Contract.Authentication;

namespace PharmaGO.Application.IntegrationTests.Authentication.Employee;

public class LoginTests(PostgreSqlFixture dbFixture, EnviromentVarsFixture envVarsFixture)
    : IntegrationTestBase(dbFixture, envVarsFixture)
{
    [Fact]
    public async Task Login_WhenValidCredentials_ShouldReturnUserData()
    {
        const string email = "login@test.com";
        const string password = "Employee@123";

        await RegisterEmployeeAsync(email, password, TestPharmacy.Id);

        var loginCommand = new { Email = email, Password = password };

        var response = await HttpClient.PostAsJsonAsync("/api/auth/admin/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        content.Should().NotBeNull();
        content.Id.Should().NotBeEmpty();
        content.FirstName.Should().NotBeNullOrEmpty();
        content.LastName.Should().NotBeNullOrEmpty();
        content.Email.Should().NotBeNullOrEmpty();
        content.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WhenInvalidPassword_ShouldReturnUnauthorized()
    {
        const string email = "test@test.com";

        await RegisterEmployeeAsync(email, "CorrectPassword@123", TestPharmacy.Id);

        var loginCommand = new { Email = email, Password = "WrongPassword@123" };

        var response = await HttpClient.PostAsJsonAsync("/api/auth/admin/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task RegisterEmployeeAsync(string email, string password, Guid pharmacyId)
    {
        var httpClient = GetAuthorizedAdmin();

        var registerCommand = new
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User",
            Phone = "48999999999",
            IsAdmin = false
        };

        var response =
            await httpClient.PostAsJsonAsync($"/api/auth/admin/register?pharmacyId={pharmacyId}", registerCommand);
        response.EnsureSuccessStatusCode();
    }
}