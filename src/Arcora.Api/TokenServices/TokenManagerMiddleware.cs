// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.Net;
using Arcora.Api.Entities;
using Microsoft.AspNetCore.Http;

namespace Arcora.Api.TokenServices
{
    public class TokenManagerMiddleware : IMiddleware
    {
        private readonly ITokenService tokenService;
        public TokenManagerMiddleware(ITokenService tokenManager)
        {
            this.tokenService = tokenManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (await this.tokenService.IsCurrentActiveToken())
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}