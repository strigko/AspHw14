using FluentValidation;

namespace p48_lessons_asp.Models.UserRequestModels;

public class LoginUserModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginUserValidator : AbstractValidator<LoginUserModel>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
