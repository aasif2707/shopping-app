using FluentValidation;

namespace IdentityManagement.Application.Features.Authentication.Commands.Account;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Token).NotEmpty();
    }
}

public class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
    }
}

public class SendPhoneOtpCommandValidator : AbstractValidator<SendPhoneOtpCommand>
{
    public SendPhoneOtpCommandValidator()
    {
        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{6,14}$");
    }
}

public class ConfirmPhoneOtpCommandValidator : AbstractValidator<ConfirmPhoneOtpCommand>
{
    public ConfirmPhoneOtpCommandValidator()
    {
        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{6,14}$");
        RuleFor(x => x.Request.Otp).NotEmpty().Length(6);
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Token).NotEmpty();
        RuleFor(x => x.Request.NewPassword)
            .NotEmpty()
            .MinimumLength(12)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
        RuleFor(x => x.Request.ConfirmPassword)
            .Equal(x => x.Request.NewPassword);
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request.CurrentPassword).NotEmpty();
        RuleFor(x => x.Request.NewPassword)
            .NotEmpty()
            .MinimumLength(12)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
        RuleFor(x => x.Request.ConfirmPassword)
            .Equal(x => x.Request.NewPassword);
    }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Address).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Request.Address));
    }
}

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.Request.NewEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.CurrentPassword).NotEmpty();
    }
}

public class ChangePhoneNumberCommandValidator : AbstractValidator<ChangePhoneNumberCommand>
{
    public ChangePhoneNumberCommandValidator()
    {
        RuleFor(x => x.Request.NewPhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{6,14}$");
        RuleFor(x => x.Request.Otp).NotEmpty().Length(6);
    }
}

public class SetProfileImageCommandValidator : AbstractValidator<SetProfileImageCommand>
{
    public SetProfileImageCommandValidator()
    {
        RuleFor(x => x.Request.ProfileImageUrl).NotEmpty().MaximumLength(1000);
    }
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
    }
}
