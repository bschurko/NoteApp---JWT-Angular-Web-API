using System.Net.WebSockets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoteApp.Server.Data;
using NoteApp.Server.Models;

namespace NoteApp.Server.Services
{
    public class AuthService
    {
        private readonly AuthDbContext _authContext;
        
        public AuthService(AuthDbContext context)
        {
            _authContext = context;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string id)
        {
            var result = await _authContext.IdentityUsers.FirstOrDefaultAsync(cc => cc.Id.Equals(id));
            return result;
        }
        public async Task<IdentityUser> GetUserByUserNameAsync(string username)
        {
            var result = await _authContext.IdentityUsers.FirstOrDefaultAsync(cc => cc.UserName.Equals(username));
            return result;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var result = await _authContext.IdentityUsers.FirstOrDefaultAsync(cc => cc.Email.Equals(email));
            return result;
        }
    }
}
