using FluentValidation;
using MyWebApp.DTOs;

namespace MyWebApp.Validators;

public class RegisterUserRequestValidator
    : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
          .MinimumLength(2)
          .MaximumLength(50);

        RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          .MaximumLength(255);

        RuleFor(x => x.Age)
          .InclusiveBetween(4, 120);

        RuleFor(x => x.Password)
          .NotEmpty()
          .MinimumLength(4);
    }
}