using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PharmaGO.Application.IntegrationTests.Infrastructure;
using PharmaGO.Application.IntegrationTests.Infrastructure.Fixtures;
using PharmaGO.Contract.Product;

namespace PharmaGO.Application.IntegrationTests.Products;

public class CreateProductTests(PostgreSqlFixture dbFixture, EnviromentVarsFixture envVarsFixture)
    : IntegrationTestBase(dbFixture, envVarsFixture)
{
    [Fact]
    public async Task CreateProduct_WhenAuthenticated_ShouldPersistToDatabase()
    {
        var httpClient = GetAuthorizedEmployee();

        var createCommand = new
        {
            Name = "Aspirina",
            Price = 10.50m,
            Category = "Health",
            Description = "Teste",
            Image = ""
        };

        var response = await httpClient.PostAsJsonAsync($"/api/products/{TestPharmacy.Id}", createCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productData = await response.Content.ReadFromJsonAsync<ProductResponse>();
        productData.Should().NotBeNull();

        await using var context = Context;
        var product = await context.Products.FindAsync(productData.Id);

        product.Should().NotBeNull();
        product!.Name.Should().Be("Aspirina");
        product.Price.Should().Be(10.50m);
    }

    [Fact]
    public async Task CreateProduct_WithoutAuthentication_ShouldReturnForbidden()
    {
        var createCommand = new
        {
            Name = "Aspirina",
            Price = 10.50m,
            Category = "Health",
        };

        var response = await HttpClient.PostAsJsonAsync($"/api/products/{TestPharmacy.Id}", createCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}