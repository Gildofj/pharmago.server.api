using ErrorOr;
using MediatR;
using PharmaGO.Core.Entities;
using static PharmaGO.Core.Common.Constants.ProductConstans;

namespace PharmaGO.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    decimal Price,
    string Description,
    Category Category,
    string Image,
    Guid PharmacyId
) : IRequest<ErrorOr<Product>>;