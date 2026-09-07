// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Arcora.Api.Models;
using Arcora.Api.Configurations;
using Arcora.Api.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Arcora.Api.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JwtConfiguration> jwtSetting;
        private readonly IMemoryCache cache;
        private readonly IHttpContextAccessor httpContextAccessor;
        public TokenService(IMemoryCache cache, IOptions<JwtConfiguration> jwtSetting, IHttpContextAccessor httpContextAccessor)
        {
            this.cache = cache;
            this.jwtSetting = jwtSetting;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GenerateJwtToken(User user, List<string> roles)
        {
            IEnumerable<Claim> userRegisteredClaims = UserClaimsFromRoles(user, roles);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.AddDays(2).ToString()),
                new Claim(ClaimTypes.Email, user.Email!.ToString()),
                new Claim(ClaimTypes.Name, $"{user.LastName}-{user.FirstName}-{user.UserName}"),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
            }.Union(userRegisteredClaims);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSetting.Value.Key!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var expires = now.AddMonths(Convert.ToInt16(this.jwtSetting.Value.Lifespan));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = this.jwtSetting.Value.Audience,
                Issuer = this.jwtSetting.Value.Issuer,
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                Expires = expires,
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static IEnumerable<Claim> UserClaimsFromRoles(User user, List<string> roles)
        {
            var claims = new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                switch (role)
                {
                    case "Admin":
                        claims.Add(new Claim("Permission", "CanManageUsers"));
                        claims.Add(new Claim("Permission", "CanDelete"));
                        break;
                    case "User":
                        claims.Add(new Claim("Permission", "CanCreate"));
                        claims.Add(new Claim("Permission", "CanEdit"));
                        break;
                    case "Viewer":
                        claims.Add(new Claim("Permission", "CanView"));
                        break;
                }
            }

            claims.Add(new Claim("UserId", user.Id.ToString()));
            claims.Add(new Claim("Username", user.UserName ?? string.Empty));
            claims.Add(new Claim("Email", user.Email ?? string.Empty));
            return claims;
        }

        public static string encodeJWTToken(string token, string email, bool twoWayAuthentication, int otpLifeTime)
        {
            Random rnd = new Random();
            string myOTP = rnd.Next(12345, 100000).ToString();
            DateTime time = DateTime.Now.AddMinutes(otpLifeTime);
            var otpExpiryTime = Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(time.ToString()))));
            var plainTextBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            var encodeEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainTextBytes));
            if (!twoWayAuthentication)
                return encodeEncoded;
            string OTP = Convert.ToBase64String(Encoding.UTF8.GetBytes(myOTP));
            return $"{encodeEncoded}-DEFmkrTUpW0616384oAHT-{OTP}-DEFmkrTUpW0616384oAHT-{otpExpiryTime}";
        }

        public async Task<bool> IsCurrentActiveToken() => await IsActiveAsync(GetCurrentAsync());
        public async System.Threading.Tasks.Task DeactivateCurrentAsync() => await DeactivateAsync(GetCurrentAsync());
        public Task<bool> IsActiveAsync(string token)
        {
            var key = GetKey(token);
            return System.Threading.Tasks.Task.FromResult(!this.cache.TryGetValue(key, out _));
        }

        public System.Threading.Tasks.Task DeactivateAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var key = GetKey(token);
                this.cache.Set(key, "deactivated", new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(jwtSetting.Value.Lifespan) });
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        private string GetCurrentAsync()
        {
            var authorizationHeader = this.httpContextAccessor.HttpContext!.Request.Headers["authorization"];
            return authorizationHeader == StringValues.Empty ? string.Empty : authorizationHeader.Single()!.Split(" ").Last();
        }

        private static string GetKey(string token) => $"tokens:{token}:deactivated";
        private Task<string?> GetDeactivatedTokenAsync(string token)
        {
            var key = GetKey(token);
            if (this.cache.TryGetValue(key, out string? value))
                return System.Threading.Tasks.Task.FromResult(value);
            return System.Threading.Tasks.Task.FromResult<string?>(null);
        }

        public Task<bool> ConfirmDeactivatedTokenAsync()
        {
            var res = GetDeactivatedTokenAsync(GetCurrentAsync()).Result;
            return System.Threading.Tasks.Task.Run(() => res == "Deactivated");
        }
    }
}