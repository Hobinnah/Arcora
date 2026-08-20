// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using Arcora.Api.Entities;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.TokenServices;

namespace Arcora.Api.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signIn;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender? _email;
        private readonly IConfiguration _configuration;
        private readonly IPreferenceService _preferenceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signIn, ITokenService tokenService, IOptions<IdentityOptions> identityOptions, IHttpContextAccessor httpContextAccessor, IEmailSender? email, IConfiguration configuration, IPreferenceService preferenceService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signIn = signIn;
            _tokenService = tokenService;
            _email = email;
            _configuration = configuration;
            _preferenceService = preferenceService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);
            if (user is null)
                return Result<AuthResponse>.Fail("Invalid credentials.");
            var check = await _signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (check.IsLockedOut)
                return Result<AuthResponse>.Fail("Account locked. Try again later.");
            if (check.IsNotAllowed)
                return Result<AuthResponse>.Fail("Sign-in not allowed (email not confirmed or policy blocked).");
            if (!check.Succeeded)
                return Result<AuthResponse>.Fail("Invalid credentials.");
            var roles = await _userManager.GetRolesAsync(user);
            var token = TokenService.encodeJWTToken(this._tokenService.GenerateJwtToken(user, roles.ToList()), user.Email!, user.TwoFactorEnabled, 10);
            var resp = new AuthResponse(IsLoginSuccessful: true, AccessToken: token, Roles: roles, User: user);
            return Result<AuthResponse>.Ok(resp);
        }

        /// <inheritdoc/>
        public async Task LogoutAsync(CancellationToken ct = default)
        {
            await _signIn.Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _tokenService.DeactivateCurrentAsync();
        }

        /// <inheritdoc/>
        public async Task<Result> RegisterAsync(RegisterRequest request, IEnumerable<string>? roles = null, bool signIn = false, CancellationToken ct = default)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.EmailAddress,
                Email = request.EmailAddress,
                TwoFactorEnabled = false,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 3,
                DisplayName = $"{request.FirstName} {request.LastName}",
                ClientName = "",
                BranchName = ""
            };
            if (roles is null || !roles.Any())
                roles = new List<string>
                {
                    "User"
                };
            var created = await _userManager.CreateAsync(user, request.Password);
            if (!created.Succeeded)
                return Result.FromIdentity(created);
            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new Role { Name = roleName, Enabled = true });
                    if (!roleResult.Succeeded)
                        return Result.FromIdentity(roleResult);
                }
            }

            var r = await _userManager.AddToRolesAsync(user, roles);
            if (!r.Succeeded)
                return Result.FromIdentity(r);
            if (signIn)
                await _signIn.SignInAsync(user, isPersistent: false);
            return Result.Ok();
        }

        /// <inheritdoc/>
        public async Task<Result> CreateRoleAsync(string roleName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Result.Fail("Role name cannot be empty.");
            if (await _roleManager.RoleExistsAsync(roleName))
                return Result.Fail("Role already exists.");
            var result = await _roleManager.CreateAsync(new Role { Name = roleName, Enabled = true });
            return Result.FromIdentity(result);
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetUsersAsync(CancellationToken ct = default)
        {
            await Task.CompletedTask;
            return _userManager.Users?.ToList() ?? new List<User>();
        }

        /// <inheritdoc/>
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Fail("User not found.");
            var r = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result> ForgotPasswordAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Result.Ok();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var baseUrl = _configuration["ApiHostUrl"]?.TrimEnd('/') ?? throw new InvalidOperationException("ApiHostUrl is not configured in appsettings.");
            var resetLink = $"{baseUrl}/api/Account/ResetPassword?email={WebUtility.UrlEncode(email)}&token={encodedToken}";
            if (_email is not null)
                await _email.SendEmailAsync(email, "Reset your password", $"Click the link to reset your password: {WebUtility.HtmlEncode(resetLink)}");
            return Result.Ok();
        }

        /// <inheritdoc/>
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);
            if (user is null)
                return Result.Ok();
            var r = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Fail("User not found.");
            var r = await _userManager.ConfirmEmailAsync(user, token);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result<string>.Fail("User not found.");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result<string>.Ok(token);
        }

        private async Task<(string CompanyName, string CompanyEmail)> GetCompanyInfoAsync()
        {
            try
            {
                var preference = await _preferenceService.GetPreference();
                if (preference != null)
                    return (preference.CompanyName ?? "", preference.CompanyEmail ?? "");
            }
            catch
            { /* non-critical */
            }

            return ("", "");
        }

        /// <inheritdoc/>
        public async Task<Result<AuthResponse>> HandleExternalLoginAsync(string provider, CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return Result<AuthResponse>.Fail("HTTP context is not available.");
            var info = await _signIn.GetExternalLoginInfoAsync();
            if (info == null)
            {
                var authenticateResult = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
                if (!authenticateResult.Succeeded)
                {
                    authenticateResult = await httpContext.AuthenticateAsync(provider);
                    if (!authenticateResult.Succeeded)
                        return Result<AuthResponse>.Fail("Error loading external login information.");
                }

                var principal = authenticateResult.Principal;
                var providerKey = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(providerKey))
                    return Result<AuthResponse>.Fail("External provider did not return a user identifier.");
                info = new ExternalLoginInfo(principal, provider, providerKey, provider);
            }

            var signInResult = await _signIn.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Result<AuthResponse>.Fail("Email not provided by external provider.");
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result<AuthResponse>.Fail("User not found.");
                var roles = await _userManager.GetRolesAsync(user);
                var token = TokenService.encodeJWTToken(_tokenService.GenerateJwtToken(user, roles.ToList()), user.Email!, user.TwoFactorEnabled, 10);
                return Result<AuthResponse>.Ok(new AuthResponse(IsLoginSuccessful: true, AccessToken: token, Roles: roles, User: user));
            }

            var email2 = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email2))
                return Result<AuthResponse>.Fail("Email not provided by external provider.");
            var existingUser = await _userManager.FindByEmailAsync(email2);
            if (existingUser != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                if (!addLoginResult.Succeeded)
                    return Result<AuthResponse>.Fail("Failed to link external login to existing account.");
                await _signIn.SignInAsync(existingUser, isPersistent: false);
                var roles2 = await _userManager.GetRolesAsync(existingUser);
                var token2 = TokenService.encodeJWTToken(_tokenService.GenerateJwtToken(existingUser, roles2.ToList()), existingUser.Email!, existingUser.TwoFactorEnabled, 10);
                return Result<AuthResponse>.Ok(new AuthResponse(IsLoginSuccessful: true, AccessToken: token2, Roles: roles2, User: existingUser));
            }

            var givenName = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            var surname = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
            var name = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var newUser = new User
            {
                UserName = email2,
                Email = email2,
                EmailConfirmed = true,
                FirstName = givenName ?? name ?? email2.Split('@')[0],
                LastName = surname ?? "",
                DisplayName = name ?? email2,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                ClientName = "",
                BranchName = ""
            };
            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
                return Result<AuthResponse>.Fail(string.Join(", ", createResult.Errors.Select(e => e.Description)));
            const string defaultRole = "User";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
                await _roleManager.CreateAsync(new Role { Name = defaultRole, Enabled = true });
            await _userManager.AddToRoleAsync(newUser, defaultRole);
            await _userManager.AddLoginAsync(newUser, info);
            await _signIn.SignInAsync(newUser, isPersistent: false);
            var roles3 = await _userManager.GetRolesAsync(newUser);
            var token3 = TokenService.encodeJWTToken(_tokenService.GenerateJwtToken(newUser, roles3.ToList()), newUser.Email!, newUser.TwoFactorEnabled, 10);
            return Result<AuthResponse>.Ok(new AuthResponse(IsLoginSuccessful: true, AccessToken: token3, Roles: roles3, User: newUser));
        }
    }
}