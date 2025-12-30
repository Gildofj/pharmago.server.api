namespace PharmaGO.Contract.Product;

public record CreateProductRequest(
    string Name,
    decimal Price,
    string Description,
    string Category,
    string Image
)
{
    public Guid PharmacyId { get; set; }
}