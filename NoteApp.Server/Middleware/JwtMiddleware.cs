using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using NoteApp.Server.Services;

namespace NoteApp.Server.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        
        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, AuthService authService)
        {
            _next = next;
            _configuration = configuration;
            _authService = authService;
        }

        public async Task Invoke(HttpContext context) 
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachUserToContext(context, token).ConfigureAwait(false);

            await _next(context).ConfigureAwait(false);
        }

        private async Task attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    
                    ValidAudience = _configuration["JwtConfig:Audience"] ?? "NoteApp.Server",
                    ValidateLifetime = true,
                    LifetimeValidator = (notBefore, expires, token, parameters) =>
                    {
                        // Custom logic: allow expired tokens for 30 minutes
                        return expires != null && 
                               expires > DateTime.UtcNow.AddMinutes(-1 * int.Parse(_configuration["JwtConfig:TokenValidityMins"] ?? "30"));
                    },
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == "sub").Value;

                Console.WriteLine("Issuer: " + jwtToken.Issuer);
                Console.WriteLine("Audience: " + jwtToken.Audiences.FirstOrDefault());
                Console.WriteLine("Expires: " + jwtToken.ValidTo);

                // attach user to context on successful jwt validation

                var currentUser = await _authService.GetUserByUserNameAsync(username).ConfigureAwait(false);

                context.Items["User"] = currentUser;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
