// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Arcora.Api.Entities;
using Arcora.Api.Accounts;
using Arcora.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login([FromServices] IAccountService accountService, [FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await accountService.LoginAsync(request, ct);
            if (!result.Succeeded)
                return Unauthorized(result.Errors);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromServices] IAccountService accountService, CancellationToken ct)
        {
            await accountService.LogoutAsync(ct);
            return NoContent();
        }

        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Register([FromServices] IAccountService accountService, [FromBody] RegisterRequest request, [FromQuery] bool signIn = false, CancellationToken ct = default)
        {
            var result = await accountService.RegisterAsync(request, request.Roles, signIn, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRoles([FromServices] IAccountService accountService, [FromBody] List<string> roles, CancellationToken ct = default)
        {
            foreach (var item in roles)
            {
                var result = await accountService.CreateRoleAsync(item, ct);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <param name = "ct"></param>
        /// <returns></returns>
        [Authorize(Roles = "User, Viewer, LandLord, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromServices] IAccountService accountService, CancellationToken ct = default)
        {
            var users = new List<User>();
            var userList = await accountService.GetUsersAsync(ct);
            foreach (var item in userList)
            {
                users.Add(new User { Id = item.Id, UserName = item.UserName, DisplayName = $"{item.FirstName} {item.LastName}" });
            }

            return Ok(users);
        }

        [Authorize(Roles = "User, Viewer, LandLord, Admin")]
        [HttpPost]
        [Route("/api/account/changePassword")]
        public async Task<IActionResult> ChangePassword([FromServices] IAccountService accountService, [FromServices] UserManager<User> userManager, [FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser is null)
                return Unauthorized();
            var result = await accountService.ChangePasswordAsync(currentUser.Id.ToString(), request, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
        }

        [Authorize]
        [HttpGet("/api/account/profile")]
        public async Task<IActionResult> GetProfile([FromServices] UserManager<User> userManager)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();
            return Ok(new
            {
                success = true,
                message = "Profile loaded.",
                data = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    phone = user.PhoneNumber,
                    user.EmailConfirmed,
                    user.PhoneNumberConfirmed
                }
            });
        }

        [Authorize]
        [HttpPut("/api/account/profile")]
        public async Task<IActionResult> UpdateProfile([FromServices] UserManager<User> userManager, [FromBody] AccountProfileUpdateDto request)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            if (request.FirstName is not null)
                user.FirstName = request.FirstName.Trim();
            if (request.LastName is not null)
                user.LastName = request.LastName.Trim();
            if (request.Phone is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Phone))
                {
                    user.PhoneNumber = null;
                }
                else if (!PhoneNumberNormalizer.TryNormalize(request.Phone, out var normalizedPhoneNumber))
                {
                    return BadRequest(new[] { PhoneNumberNormalizer.InvalidPhoneNumberError });
                }
                else
                {
                    if (await userManager.Users.AnyAsync(
                        otherUser => otherUser.Id != user.Id && otherUser.PhoneNumber == normalizedPhoneNumber))
                        return Conflict(new[] { PhoneNumberNormalizer.DuplicatePhoneNumberError });

                    user.PhoneNumber = normalizedPhoneNumber;
                }
            }
            user.DisplayName = $"{user.FirstName} {user.LastName}".Trim();

            IdentityResult result;
            try
            {
                result = await userManager.UpdateAsync(user);
            }
            catch (DbUpdateException exception) when (PhoneNumberNormalizer.IsPhoneNumberUniqueConstraintViolation(exception))
            {
                return Conflict(new[] { PhoneNumberNormalizer.DuplicatePhoneNumberError });
            }
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                success = true,
                message = "Profile updated.",
                data = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    phone = user.PhoneNumber,
                    user.EmailConfirmed,
                    user.PhoneNumberConfirmed
                }
            });
        }

        [Authorize]
        [HttpGet("/api/account/settings")]
        public async Task<IActionResult> GetSettings([FromServices] ArcoraDbContext db, [FromServices] UserManager<User> userManager)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            var settings = await db.AccountSettings.AsNoTracking().SingleOrDefaultAsync(item => item.UserID == user.Id);
            return Ok(new
            {
                success = true,
                message = "Account settings loaded.",
                data = settings is null
                    ? new AccountSettingsDto()
                    : new AccountSettingsDto
                {
                    EmailNotifications = settings.EmailNotifications,
                    SmsNotifications = settings.SmsNotifications,
                    Language = settings.Language,
                    Timezone = settings.Timezone,
                    Currency = settings.Currency
                }
            });
        }

        [Authorize]
        [HttpPut("/api/account/settings")]
        public async Task<IActionResult> UpdateSettings([FromServices] ArcoraDbContext db, [FromServices] UserManager<User> userManager, [FromBody] AccountSettingsDto request)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            var settings = await db.AccountSettings.SingleOrDefaultAsync(item => item.UserID == user.Id);
            if (settings is null)
            {
                settings = new AccountSettings { UserID = user.Id };
                db.AccountSettings.Add(settings);
            }

            settings.EmailNotifications = request.EmailNotifications;
            settings.SmsNotifications = request.SmsNotifications;
            settings.Language = request.Language.Trim();
            settings.Timezone = request.Timezone.Trim();
            settings.Currency = request.Currency.Trim().ToUpperInvariant();
            settings.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Account settings updated.",
                data = new AccountSettingsDto
                {
                    EmailNotifications = settings.EmailNotifications,
                    SmsNotifications = settings.SmsNotifications,
                    Language = settings.Language,
                    Timezone = settings.Timezone,
                    Currency = settings.Currency
                }
            });
        }

        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> ForgotPassword([FromServices] IAccountService accountService, [FromQuery] string email, CancellationToken ct)
        {
            var result = await accountService.ForgotPasswordAsync(email, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> ResetPassword([FromServices] IAccountService accountService, [FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            var result = await accountService.ResetPasswordAsync(request, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromServices] IAccountService accountService, [FromQuery] string userId, [FromQuery] string token, CancellationToken ct)
        {
            var result = await accountService.ConfirmEmailAsync(userId, token, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromServices] IAccountService accountService, [FromQuery] string userId, CancellationToken ct)
        {
            var result = await accountService.GenerateEmailConfirmationTokenAsync(userId, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(result.Value);
        }

        /// <summary>
        /// Checks whether the supplied email is already registered. When the email is new,
        /// a short-lived verification code is emailed to begin the sign-up workflow.
        /// </summary>
        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> RequestLoginCode([FromServices] IAccountService accountService, [FromBody] RequestLoginCodeRequest request, CancellationToken ct)
        {
            var result = await accountService.RequestLoginCodeAsync(request, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(result.Value);
        }

        /// <summary>
        /// Verifies the code emailed to the user and, on success, returns an authenticated token.
        /// </summary>
        [HttpPost]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> VerifyLoginCode([FromServices] IAccountService accountService, [FromBody] VerifyLoginCodeRequest request, CancellationToken ct)
        {
            var result = await accountService.VerifyLoginCodeAsync(request, ct);
            if (!result.Succeeded)
                return Unauthorized(result.Errors);
            return Ok(result.Value);
        }

        /// <summary>
        /// Initiates Google OAuth login flow
        /// </summary>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl }, Request.Scheme)
            };
            return Challenge(properties, "Google");
        }

        /// <summary>
        /// Handles the callback from Google OAuth
        /// </summary>
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromServices] IAccountService accountService, [FromQuery] string? returnUrl = null, CancellationToken ct = default)
        {
            var result = await accountService.HandleExternalLoginAsync("Google", ct);
            if (!result.Succeeded)
            {
                // Encode error response and redirect to frontend
                var errorResponse = new
                {
                    success = false,
                    errors = result.Errors
                };
                var errorJson = JsonSerializer.Serialize(errorResponse);
                var errorEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(errorJson));
                var frontendUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["FrontendUrl"] ?? "http://localhost:5173";
                return Redirect($"{frontendUrl}/auth/callback?response={errorEncoded}");
            }

            // Encode success response and redirect to frontend
            var authResponse = new
            {
                success = true,
                isLoginSuccessful = result.Value!.IsLoginSuccessful,
                accessToken = result.Value.AccessToken,
                roles = result.Value.Roles,
                user = result.Value.User
            };
            var json = JsonSerializer.Serialize(authResponse);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(json));
            var frontendUrlSuccess = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["FrontendUrl"] ?? "http://localhost:5173";
            return Redirect($"{frontendUrlSuccess}/auth/callback?response={encoded}");
        }
    }
}