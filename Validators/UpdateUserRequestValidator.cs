using FluentValidation;
using MyWebApp.DTOs;

namespace MyWebApp.Validators;

public class UpdateUserRequestValidator
    : AbstractValidator<CreateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.Age)
            .InclusiveBetween(1, 120);
    }
}