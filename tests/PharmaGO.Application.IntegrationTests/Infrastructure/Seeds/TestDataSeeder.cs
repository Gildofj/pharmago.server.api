using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmaGO.Application.Common.Auth.Constants;
using PharmaGO.Core.Entities;
using PharmaGO.Core.ValueObjects;
using PharmaGO.Infrastructure.Persistence;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Seeds;

public class TestDataSeeder(PharmaGOContext context, IConfiguration configuration)
{
    public async Task<Pharmacy> SeedAsync()
    {
        var pharmacy = await SeedPharmacyAsync();
        await SeedRolesAsync();
        await SeedAdminUserAsync();

        return pharmacy;
    }

    private async Task<Pharmacy> SeedPharmacyAsync()
    {
        if (await context.Pharmacies.FirstOrDefaultAsync(p => p.Cnpj == "12.345.678/0001-99") 
            is { } existingPharmacy)
        {
            return existingPharmacy;
        }

        var address = Address.Create(
            street: "Rodovia Francisco Thomaz dos Santos",
            number: "1234",
            neighborhood: "Armação do Pantano do Sul",
            city: "Florianópolis",
            state: "SC",
            country: "Brasil",
            zipCode: "88066000"
        );

        var pharmacy = new Pharmacy
        {
            Id = Guid.NewGuid(),
            Name = "PharmaGO Demo",
            Cnpj = "12.345.678/0001-99",
            ContactNumber = "12345678901",
            Address = address.Value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        context.Pharmacies.Add(pharmacy);
        await context.SaveChangesAsync();
        
        return pharmacy;
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] 
        { 
            EmployeeRoles.Admin, 
            EmployeeRoles.Employee, 
        };

        foreach (var roleName in roles)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == roleName))
            {
                context.Roles.Add(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = configuration["AdminUser:Email"] 
            ?? throw new InvalidOperationException("AdminUser:Email not configured");
        var adminPassword = configuration["AdminUser:Password"] 
            ?? throw new InvalidOperationException("AdminUser:Password not configured");

        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (existingUser != null)
        {
            return;
        }

        var pharmacy = await context.Pharmacies.FirstAsync(p => p.Cnpj == "12.345.678/0001-99");
        var adminRole = await context.Roles.FirstAsync(r => r.Name == EmployeeRoles.Admin);
        
        var adminUserId = Guid.NewGuid();
        var passwordHasher = new PasswordHasher<IdentityUser<Guid>>();
        var hashedPassword = passwordHasher.HashPassword(null!, adminPassword);

        // Criar usuário Identity
        var adminUser = new IdentityUser<Guid>
        {
            Id = adminUserId,
            UserName = adminEmail,
            NormalizedUserName = adminEmail.ToUpperInvariant(),
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            PasswordHash = hashedPassword,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        context.Users.Add(adminUser);

        // Adicionar role
        context.UserRoles.Add(new IdentityUserRole<Guid>
        {
            UserId = adminUserId,
            RoleId = adminRole.Id
        });

        // Criar Employee
        var employee = new Employee
        {
            Id = adminUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FirstName = "Admin",
            LastName = "Demo",
            Email = Email.Create(adminEmail).Value,
            Phone = "(99) 99999-9999",
            PharmacyId = pharmacy.Id,
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();
    }

    public async Task<Employee> CreateEmployeeAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string role,
        Guid? pharmacyId = null)
    {
        var pharmacy = pharmacyId.HasValue
            ? await context.Pharmacies.FindAsync(pharmacyId.Value)
            : await context.Pharmacies.FirstAsync();

        if (pharmacy == null)
        {
            throw new InvalidOperationException("Pharmacy not found");
        }

        var roleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Name == role)
            ?? throw new InvalidOperationException($"Role '{role}' not found");

        var employeeId = Guid.NewGuid();
        var passwordHasher = new PasswordHasher<IdentityUser<Guid>>();
        var hashedPassword = passwordHasher.HashPassword(null!, password);

        var user = new IdentityUser<Guid>
        {
            Id = employeeId,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            PasswordHash = hashedPassword,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        context.Users.Add(user);

        context.UserRoles.Add(new IdentityUserRole<Guid>
        {
            UserId = employeeId,
            RoleId = roleEntity.Id
        });

        var employee = new Employee
        {
            Id = employeeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FirstName = firstName,
            LastName = lastName,
            Email = Email.Create(email).Value,
            Phone = "(48) 98888-8888",
            PharmacyId = pharmacy.Id,
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        return employee;
    }
}
