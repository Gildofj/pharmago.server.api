using ErrorOr;
using MediatR;
using PharmaGO.Application.Common.Interfaces;
using PharmaGO.Core.Interfaces.Persistence;
using PharmaGO.Core.Entities;
using PharmaGO.Core.Common.Errors;

namespace PharmaGO.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
)
    : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    public async Task<ErrorOr<Product>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(
            name: command.Name,
            price: command.Price,
            description: command.Description,
            image: command.Image,
            category: command.Category,
            pharmacyId: command.PharmacyId
        );

        if (productResult.IsError)
        {
            return productResult.Errors;
        }

        var product = productResult.Value;

        await productRepository.AddAsync(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product;
    }
}