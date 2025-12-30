using System.Text.RegularExpressions;
using ErrorOr;
using PharmaGO.Core.Common;
using PharmaGO.Core.Common.Errors;

namespace PharmaGO.Core.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250)
    );

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static ErrorOr<Email> Create(string? email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            return Errors.Email.Empty;
        }
        
        email = email.Trim().ToLowerInvariant();

        if (email.Length > 255)
        {
            return Errors.Email.TooLong;
        }

        if (!EmailRegex.IsMatch(email))
        {
            return Errors.Email.InvalidFormat;
        }
        
        var parts = email.Split('@');
        if (parts[0].Length > 64)
        {
            return Errors.Email.LocalPartTooLong;
        }

        return new Email(email);
    }

    public static implicit operator string(Email email) => email.Value;
    
    public override string ToString() => Value;

    protected override IEnumerable<object?> GetAtomicValues() => [Value];
}