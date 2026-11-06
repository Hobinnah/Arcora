using Arcora.Api.Accounts;
using Arcora.Api.DTOs;

namespace Arcora.Api.Services.Interfaces;

public interface IPhoneVerificationService
{
    Task<Result<PhoneVerificationStatusDto>> SendVerificationCodeAsync(long userId, CancellationToken cancellationToken = default);
    Task<Result<PhoneVerificationStatusDto>> ConfirmPhoneNumberAsync(long userId, string code, CancellationToken cancellationToken = default);
}
