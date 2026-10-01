using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Models;
using IdentityManagement.Application.Features.Authentication.Dtos;
using MediatR;

namespace IdentityManagement.Application.Features.Authentication.Commands.Account;

public record ConfirmEmailCommand(ConfirmEmailRequestDto Request) : IRequest<Result>;
public record ResendEmailConfirmationCommand(ResendEmailConfirmationRequestDto Request) : IRequest<Result>;
public record SendPhoneOtpCommand(SendPhoneOtpRequestDto Request) : IRequest<Result>;
public record ConfirmPhoneOtpCommand(ConfirmPhoneOtpRequestDto Request) : IRequest<Result>;
public record ForgotPasswordCommand(ForgotPasswordRequestDto Request) : IRequest<Result>;
public record ResetPasswordCommand(ResetPasswordRequestDto Request) : IRequest<Result>;
public record ChangePasswordCommand(Guid UserId, ChangePasswordRequestDto Request) : IRequest<Result>;
public record UpdateProfileCommand(Guid UserId, UpdateProfileRequestDto Request) : IRequest<Result<UserProfileDto>>;
public record ChangeEmailCommand(Guid UserId, ChangeEmailRequestDto Request) : IRequest<Result>;
public record ChangePhoneNumberCommand(Guid UserId, ChangePhoneNumberRequestDto Request) : IRequest<Result>;
public record SetProfileImageCommand(Guid UserId, SetProfileImageRequestDto Request) : IRequest<Result>;
public record DeleteProfileImageCommand(Guid UserId) : IRequest<Result>;

public class ConfirmEmailCommandHandler(IIdentityService identityService) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        => identityService.ConfirmEmailAsync(request.Request, cancellationToken);
}

public class ResendEmailConfirmationCommandHandler(IIdentityService identityService) : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    public Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        => identityService.ResendEmailConfirmationAsync(request.Request, cancellationToken);
}

public class SendPhoneOtpCommandHandler(IIdentityService identityService) : IRequestHandler<SendPhoneOtpCommand, Result>
{
    public Task<Result> Handle(SendPhoneOtpCommand request, CancellationToken cancellationToken)
        => identityService.SendPhoneOtpAsync(request.Request, cancellationToken);
}

public class ConfirmPhoneOtpCommandHandler(IIdentityService identityService) : IRequestHandler<ConfirmPhoneOtpCommand, Result>
{
    public Task<Result> Handle(ConfirmPhoneOtpCommand request, CancellationToken cancellationToken)
        => identityService.ConfirmPhoneOtpAsync(request.Request, cancellationToken);
}

public class ForgotPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    public Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        => identityService.ForgotPasswordAsync(request.Request, cancellationToken);
}

public class ResetPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ResetPasswordCommand, Result>
{
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        => identityService.ResetPasswordAsync(request.Request, cancellationToken);
}

public class ChangePasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ChangePasswordCommand, Result>
{
    public Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => identityService.ChangePasswordAsync(request.UserId, request.Request, cancellationToken);
}

public class UpdateProfileCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        => identityService.UpdateProfileAsync(request.UserId, request.Request, cancellationToken);
}

public class ChangeEmailCommandHandler(IIdentityService identityService) : IRequestHandler<ChangeEmailCommand, Result>
{
    public Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
        => identityService.ChangeEmailAsync(request.UserId, request.Request, cancellationToken);
}

public class ChangePhoneNumberCommandHandler(IIdentityService identityService) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    public Task<Result> Handle(ChangePhoneNumberCommand request, CancellationToken cancellationToken)
        => identityService.ChangePhoneNumberAsync(request.UserId, request.Request, cancellationToken);
}

public class SetProfileImageCommandHandler(IIdentityService identityService) : IRequestHandler<SetProfileImageCommand, Result>
{
    public Task<Result> Handle(SetProfileImageCommand request, CancellationToken cancellationToken)
        => identityService.SetProfileImageAsync(request.UserId, request.Request, cancellationToken);
}

public class DeleteProfileImageCommandHandler(IIdentityService identityService) : IRequestHandler<DeleteProfileImageCommand, Result>
{
    public Task<Result> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        => identityService.DeleteProfileImageAsync(request.UserId, cancellationToken);
}
