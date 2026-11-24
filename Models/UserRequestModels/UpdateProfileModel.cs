using FluentValidation;

namespace p48_lessons_asp.Models.UserRequestModels;

public class UpdateProfileModel
{
    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
}

public class UpdateProfileValidator : AbstractValidator<UpdateProfileModel>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(3).WithMessage("Full name must have at least 3 symbols")
            .MaximumLength(150).WithMessage("Full name must not exceed 150 symbols");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.Date).WithMessage("Date of birth must be in the past");
    }
}
