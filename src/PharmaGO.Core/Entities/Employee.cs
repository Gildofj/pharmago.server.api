using ErrorOr;
using PharmaGO.Core.Common.Errors;

namespace PharmaGO.Core.Entities;

public sealed class Employee : Person
{
    public Guid PharmacyId { get; set; } = Guid.Empty!;
    public Pharmacy Pharmacy { get; set; } = null!;

    public static ErrorOr<Employee> Create(
        string firstName,
        string lastName,
        string email,
        string phone,
        Guid pharmacyId
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
        
        if (pharmacyId == Guid.Empty)
        {
            errors.Add(Errors.Employee.PharmacyIdRequired);
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
        
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = emailResult.Value,
            Phone = phone,
            PharmacyId = pharmacyId,
        };

        return employee;
    }
}