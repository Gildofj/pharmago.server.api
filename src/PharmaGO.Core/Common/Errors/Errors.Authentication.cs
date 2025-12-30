using ErrorOr;

namespace PharmaGO.Core.Common.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials =>
            Error.Unauthorized(code: "Auth.InvalidCred", description: "Invalid credentials.");

        public static Error FirstNameNotInformed =>
            Error.Validation(code: "Auth.FirstNameNotInformed", description: "First name not informed.");

        public static Error LastNameNotInformed =>
            Error.Validation(code: "Auth.LastNameNotInformed", description: "Last name not informed.");

        public static Error PasswordNotInformed =>
            Error.Validation(code: "Auth.PasswordNotInformed", description: "Password not informed.");
    }
}