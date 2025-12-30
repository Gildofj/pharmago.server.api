using PharmaGO.Core.ValueObjects;

namespace PharmaGO.UnitTests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("USER@EXAMPLE.COM")] // Case insensitive
    public void Create_WithValidEmail_ShouldSucceed(string validEmail)
    {
        var result = Email.Create(validEmail);

        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("", "Email.Empty")]
    [InlineData(null, "Email.Empty")]
    [InlineData("   ", "Email.Empty")]
    [InlineData("invalid", "Email.InvalidFormat")]
    [InlineData("@example.com", "Email.InvalidFormat")]
    [InlineData("user@", "Email.InvalidFormat")]
    public void Create_WithInvalidEmail_ShouldReturnError(
        string? invalidEmail,
        string expectedErrorCode
    )
    {
        var result = Email.Create(invalidEmail);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(expectedErrorCode);
    }

    [Fact]
    public void Create_WithEmailTooLong_ShouldReturnError()
    {
        var longEmail = new string('a', 250) + "@example.com";

        var result = Email.Create(longEmail);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.TooLong");
        result.FirstError.Type.Should().Be(ErrorOr.ErrorType.Validation);
    }

    [Fact]
    public void Create_WithLocalPartTooLong_ShouldReturnError()
    {
        var email = new string('a', 70) + "@example.com";

        var result = Email.Create(email);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.LocalPartTooLong");
    }
}