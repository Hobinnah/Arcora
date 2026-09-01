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
        [Authorize(Roles = "User, Viewer, Admin")]
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

        [Authorize(Roles = "User, Viewer, Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IAccountService accountService, [FromQuery] string userId, [FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var result = await accountService.ChangePasswordAsync(userId, request, ct);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok();
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