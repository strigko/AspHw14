using FluentValidation;

namespace p48_lessons_asp.Models.ProductRequestModels;

public class CreateProductImageRequest
{
    public Guid ProductId {get;set;}
    public IFormFile Image{get;set;}
}

public class CreateProductImageValidator : AbstractValidator<CreateProductImageRequest>
{
    public CreateProductImageValidator()
    {
        RuleFor(img => img.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(img => img.Image).NotNull().WithMessage("Image is required");
        RuleFor(img=> img.Image.Length).LessThanOrEqualTo(5*1024*1024).When(img => img.Image != null).WithMessage("File size must be less then or equal 5MB");
    }
}