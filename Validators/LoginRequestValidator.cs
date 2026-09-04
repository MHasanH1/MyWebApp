using FluentValidation;
using MyWebApp.DTOs;

namespace MyWebApp.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress()
      .MaximumLength(255);

    RuleFor(x => x.Password)
      .NotEmpty()
      .MinimumLength(4);
  }
}