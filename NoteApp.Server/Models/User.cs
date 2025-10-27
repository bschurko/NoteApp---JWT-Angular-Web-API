using Microsoft.AspNetCore.Identity;

namespace NoteApp.Server.Models
{
    public class User : IdentityUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }

        public User(){}
        
        public User(Guid Id, string UserName, string Email, string Password, string[] Roles)
        {
            this.Id = Id;
            this.Username = UserName;
            this.Email = Email;
            this.Password = Password;
            this.Roles = Roles;
        }
    }
}
