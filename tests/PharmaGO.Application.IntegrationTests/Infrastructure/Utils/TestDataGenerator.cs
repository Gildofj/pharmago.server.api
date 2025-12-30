using Bogus;
using PharmaGO.Core.Common.Constants;
using PharmaGO.Core.Entities;

namespace PharmaGO.Application.IntegrationTests.Infrastructure.Utils;

public static class TestDataGenerator
{
    private static readonly Faker Faker = new("pt_BR");

    public static Pharmacy CreatePharmacy()
    {
        var address = Core.ValueObjects.Address.Create(
            street: Faker.Address.StreetName(),
            number: Faker.Random.AlphaNumeric(Faker.PickRandom((IEnumerable<int>)[4, 5])),
            neighborhood: Faker.Address.City(),
            city: Faker.Address.City(),
            state: Faker.Address.StateAbbr(),
            country: Faker.Address.Country(),
            zipCode: Faker.Address.ZipCode()
        ).Value;

        return Pharmacy.Create(
            name: Faker.Company.CompanyName(),
            cnpj: GenerateCnpj(),
            contactNumber: Faker.Phone.PhoneNumber(),
            address: address
        ).Value;
    }

    public static Employee CreateEmployee(Guid pharmacyId)
    {
        return Employee.Create(
            firstName: Faker.Name.FirstName(),
            lastName: Faker.Name.LastName(),
            email: Faker.Internet.Email(),
            phone: Faker.Phone.PhoneNumber("(##) #####-####"),
            pharmacyId: pharmacyId
        ).Value;
    }

    public static Client CreateClient()
    {
        return Client.Create(
            firstName: Faker.Name.FirstName(),
            lastName: Faker.Name.LastName(),
            email: Faker.Internet.Email(),
            phone: Faker.Phone.PhoneNumber("(##) ######-####"),
            cpf: GenerateCpf()
        ).Value;
    }

    public static Product CreateProduct(Guid pharmacyId)
    {
        return Product.Create(
            name: Faker.Commerce.ProductName(),
            image: Faker.Image.DataUri(200, 200),
            price: Faker.Finance.Amount(5, 500),
            description: Faker.Commerce.ProductDescription(),
            category: Faker.PickRandom<ProductConstans.Category>(),
            pharmacyId: pharmacyId
        ).Value;
    }

    private static string GenerateCpf()
    {
        return Faker.Random.ReplaceNumbers("###########");
    }

    private static string GenerateCnpj()
    {
        return Faker.Random.ReplaceNumbers("##############");
    }
}