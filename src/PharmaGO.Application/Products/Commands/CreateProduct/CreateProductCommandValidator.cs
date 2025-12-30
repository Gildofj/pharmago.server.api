using FluentValidation;

namespace PharmaGO.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
  public CreateProductCommandValidator()
  {
    RuleFor(x => x.Name).NotEmpty().NotNull();
    RuleFor(x => x.Price).NotEmpty().NotNull();
    RuleFor(x => x.Description).Length(0, 300);
  }
}