using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Server.Data;
using NoteApp.Server.Models;
using NoteApp.Server.Services;

namespace NoteApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AuthDbContext _authContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly AuthService _authService;
        
        public AccountController(ILogger<AccountController> logger, 
                AuthDbContext authContext, 
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                JwtService jwtService,
                AuthService authService)
        {
            _logger = logger;
            _authContext = authContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _authService = authService;
        }
        
        

        [Route("GetAllUsers")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IdentityUser[]>> GetAll()
        {
            var users = _authContext.IdentityUsers.ToList();
            return Ok(users);
        }
        

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                 await _signInManager.SignOutAsync();
                 return Ok();

            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }


       
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest user)
        {
 
            var identityUser = user.CreateIdentityUser();

            var canSignIn = await _signInManager.CanSignInAsync(identityUser);
            if (canSignIn)
            {
                await _signInManager.SignInAsync(identityUser, true);
                var u = await _authService.GetUserByUserNameAsync(user.UserName);
                
                var token = _jwtService.GenerateJwtToken(user.UserName);
                AuthenticationResponse response = new AuthenticationResponse(u, token);
                return Ok(response);
            }
             
            return Unauthorized();
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]AuthenticationRequest request)
        {
            var user = new IdentityUser(request.UserName);
            
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Login successful" });
            }
            else
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }
        }

         



    }
}
