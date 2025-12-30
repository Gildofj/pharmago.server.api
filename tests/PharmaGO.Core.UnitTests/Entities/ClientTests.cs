using ErrorOr;
using PharmaGO.Core.Common.Errors;
using PharmaGO.Core.Entities;
using PharmaGO.Core.ValueObjects;

namespace PharmaGO.UnitTests.Entities;

public class ClientTests
{
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "teste@example.com";
    private const string ValidCpf = "123.456.789-00";
    private const string ValidPhone = "(11) 91234-5678";

    [Fact]
    public void Create_WithValidParameters_ShouldCreateClient()
    {
        var clientResult = Client.Create(
            firstName: ValidFirstName,
            lastName: ValidLastName,
            email: ValidEmail,
            cpf: ValidCpf,
            phone: ValidPhone
        );

        clientResult.IsError.Should().BeFalse();

        var client = clientResult.Value;
        client.Should().NotBeNull();
        client.Id.Should().NotBeEmpty();
        client.FirstName.Should().Be(ValidFirstName);
        client.LastName.Should().Be(ValidLastName);
        client.Email.Should().Be(Email.Create(ValidEmail).Value);
        client.Cpf.Should().Be(ValidCpf);
        client.Phone.Should().Be(ValidPhone);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowException()
    {
        var clientResult = Client.Create(
            firstName: ValidFirstName,
            lastName: ValidLastName,
            email: "invalid-email",
            phone: ValidPhone,
            cpf: ValidCpf
        );

        clientResult.IsError.Should().BeTrue();
        clientResult.Errors.Should().HaveCount(1);
        clientResult.FirstError.Should().Be(Errors.Email.InvalidFormat);
    }
}