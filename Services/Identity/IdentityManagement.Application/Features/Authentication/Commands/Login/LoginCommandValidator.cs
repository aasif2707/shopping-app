using FluentValidation;

namespace IdentityManagement.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Request.Password).NotEmpty();
    }
}
