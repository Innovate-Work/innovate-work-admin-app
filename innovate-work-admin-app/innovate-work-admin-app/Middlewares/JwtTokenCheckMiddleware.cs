using Hanssens.Net;
using innovate_work_admin_app.Models.JWT;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

namespace innovate_work_admin_app.Middlewares
{
    public class JwtTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCheckMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, LocalStorage localStorage, IConfiguration configuration)
        {
            string tokenName = configuration.GetSection("TokenLocalStorageName").Value;
            SecurityToken? securityToken = GetSecurityToken(localStorage, tokenName);

            if(securityToken == null)
            {
                await context.SignOutAsync();
                await _next.Invoke(context);
                return;
            }

            if (securityToken.ExpireAt < DateTime.UtcNow)
            {
                localStorage.Remove(tokenName);
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            if (!context.User.Identity.IsAuthenticated && securityToken.ExpireAt > DateTime.Now)
            {
                await SignIn(securityToken, context);
            }
            await _next.Invoke(context);
        }

        private SecurityToken? GetSecurityToken(LocalStorage localStorage, string tokenName)
        {
            if (localStorage.Exists(tokenName))
            {
                string? jsonToken = localStorage.Get<string>(tokenName);
                SecurityToken? securityToken = JsonSerializer.Deserialize<SecurityToken>(jsonToken);
                return securityToken;
            }
            return null;
        }

        private async Task SignIn(SecurityToken securityToken, HttpContext context)
        {
            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, securityToken.UserName),
                new Claim(nameof(SecurityToken.Token), securityToken.Token),
                new Claim(nameof(SecurityToken.ExpireAt), securityToken.ExpireAt.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}
