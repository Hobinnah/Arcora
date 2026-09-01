// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Accounts
{
    public interface IAccountService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task LogoutAsync(CancellationToken ct = default);
        Task<Result> RegisterAsync(RegisterRequest request, IEnumerable<string>? roles = null, bool signIn = false, CancellationToken ct = default);
        Task<Result> CreateRoleAsync(string roleName, CancellationToken ct = default);
        Task<List<User>> GetUsersAsync(CancellationToken ct = default);
        Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken ct = default);
        Task<Result> ForgotPasswordAsync(string email, CancellationToken ct = default);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
        Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default);
        Task<Result<AuthResponse>> HandleExternalLoginAsync(string provider, CancellationToken ct = default);
        Task<Result<RequestLoginCodeResponse>> RequestLoginCodeAsync(RequestLoginCodeRequest request, CancellationToken ct = default);
        Task<Result<VerifyLoginCodeResponse>> VerifyLoginCodeAsync(VerifyLoginCodeRequest request, CancellationToken ct = default);
    }
}