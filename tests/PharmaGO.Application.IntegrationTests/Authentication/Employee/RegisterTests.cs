using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PharmaGO.Application.Employees.Commands.Register;
using PharmaGO.Application.IntegrationTests.Infrastructure;
using PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;
using PharmaGO.Contract.Authentication;

namespace PharmaGO.Application.IntegrationTests.Authentication.Employee;

public class RegisterTests(PostgreSqlFixture dbFixture, EnviromentVarsFixture enviromentVarsFixture)
    : IntegrationTestBase(dbFixture, enviromentVarsFixture)
{
    [Fact]
    public async Task Register_WhenValidEmployee_ShouldCreateUserAndEmployee()
    {
        var httpClient = GetAuthorizedAdmin();
        
        var registerCommand = new
        {
            Email = "employee@test.com",
            Password = "Employee@123",
            FirstName = "João",
            LastName = "Silva",
            Phone = "48999999999",
            IsAdmin = false
        };

        var response = await httpClient.PostAsJsonAsync(
            $"/api/auth/admin/register?pharmacyId={TestPharmacy.Id}",
            registerCommand
        );

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var context = Context;

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == registerCommand.Email);
        user.Should().NotBeNull();

        var employee = await context.Employees.FirstOrDefaultAsync(u => u.Email == registerCommand.Email);
        employee.Should().NotBeNull();
        employee.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Register_WhenDuplicatedEmail_ShouldReturnConflict()
    {
        var httpClient = GetAuthorizedAdmin();

        var registerCommand = new
        {
            Email = "duplicate@test.com",
            Password = "Employee@123",
            FirstName = "Test",
            LastName = "User",
            Phone = "48999999999",
            IsAdmin = false
        };

        var r = await httpClient.PostAsJsonAsync(
            $"/api/auth/admin/register?pharmacyId={TestPharmacy.Id}",
            registerCommand
        );

        var secondCommand = registerCommand with { Phone = "48777777777" };

        var response = await httpClient.PostAsJsonAsync(
            $"/api/auth/admin/register?pharmacyId={TestPharmacy.Id}",
            secondCommand
        );

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}