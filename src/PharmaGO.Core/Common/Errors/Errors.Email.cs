using ErrorOr;

namespace PharmaGO.Core.Common.Errors;

public static partial class Errors
{
    public static class Email
    {
       public static Error Empty =>
            Error.Validation(code: "Email.Empty", description: "The email was not informed.");
       
       public static Error TooLong =>
            Error.Validation(code: "Email.TooLong", description: "The email is too long.");
       
       public static Error InvalidFormat =>
            Error.Validation(code: "Email.InvalidFormat", description: "The email format is invalid.");
       
      public static Error LocalPartTooLong =>
            Error.Validation(code: "Email.LocalPartTooLong", description: "The local part of the email is too long.");
    }
}