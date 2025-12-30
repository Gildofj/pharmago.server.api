using System.ComponentModel.DataAnnotations;
using ErrorOr;
using PharmaGO.Core.Common.Errors;

namespace PharmaGO.Core.Entities;

public sealed class Client : Person
{
    public string Cpf { get; set; } = null!;

    public static ErrorOr<Client> Create(
        string firstName,
        string lastName,
        string email,
        string phone,
        string cpf
    )
    {
        List<Error> errors = [];

        if (string.IsNullOrEmpty(firstName))
        {
            errors.Add(Errors.Authentication.FirstNameNotInformed);
        }

        if (string.IsNullOrEmpty(lastName))
        {
            errors.Add(Errors.Authentication.LastNameNotInformed);
        }
        
        var emailResult = ValueObjects.Email.Create(email);

        if (emailResult.IsError)
        {
            errors.AddRange(emailResult.Errors);
        }
        
        if (errors.Count > 0)
        {
            return errors;
        }

        var employee = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = emailResult.Value,
            Phone = phone,
            Cpf = cpf,
        };

        return employee;
    }
}