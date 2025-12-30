using PharmaGO.Core.Entities.Base;
using PharmaGO.Core.ValueObjects;

namespace PharmaGO.Core.Entities;

public class Person : Entity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Email Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}