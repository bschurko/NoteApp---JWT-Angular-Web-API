using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using NoteApp.Server.Models;

namespace NoteApp.Server.Models
{
    public class AuthenticationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticationResponse
    {
        public AuthenticationResponse() {}

        public AuthenticationResponse(NoteApp.Server.Models.User user, string token)
        {
            AccessToken = token;
            UserName = user.UserName;
            Id = user.Id.ToString();

        }

        public AuthenticationResponse(IdentityUser user, string token)
        {
            AccessToken = token;
            UserName = user.UserName;
            Email = user.Email;
            Id = user.Id.ToString();

        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; } = 30;
    }
    
    public static class AuthExtensions
    {
        public static NoteApp.Server.Models.User CreateIdentityUser(this AuthenticationRequest authenticationRequest)
        {
            return new NoteApp.Server.Models.User
            {
                UserName = authenticationRequest.UserName,
                Password = authenticationRequest.Password
            };
        }
        public static AuthenticationResponse CreateLoginResponse(this AuthenticationRequest authenticationRequest, string accessToken = "")
        {
            return new NoteApp.Server.Models.AuthenticationResponse
            {
                UserName = authenticationRequest.UserName,
                ExpiresIn = 30,
                AccessToken = accessToken
            };
        }
    }
}