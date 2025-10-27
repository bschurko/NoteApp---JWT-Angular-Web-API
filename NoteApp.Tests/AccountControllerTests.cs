using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NoteApp.Server;
using NoteApp.Server.Data;
using NoteApp.Server.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace NoteApp.Tests.Integration
{

    [TestClass]
    public class AccountControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AccountControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext
                    var descriptor =
                        services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<AuthDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });

                    // Build service provider
                    var sp = services.BuildServiceProvider();

                    // Create scope and seed database
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AuthDbContext>();
                        var userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();

                        db.Database.EnsureCreated();
                        SeedTestData(db, userManager).Wait();
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        private static async Task SeedTestData(AuthDbContext db, UserManager<IdentityUser> userManager)
        {
            // Create test users
            var testUser = new IdentityUser("testuser@example.com")
            {
                Email = "testuser@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(testUser, "Test@123456");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create test user");
            }
        }

        #region Register Tests

        [TestMethod]
        public async Task Register_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
      
                UserName = "newuser@example.com",
                Password = "NewUser@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Login successful", content);
        }

        [TestMethod]
        public async Task Register_WithExistingEmail_ReturnsUnauthorized()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
     
                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Register_WithWeakPassword_ReturnsUnauthorized()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
        
                UserName = "weakpass@example.com",
                Password = "123" // Too weak
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Register_WithInvalidEmail_ReturnsUnauthorized()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
                
                UserName = "notanemail",
                Password = "ValidPass@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Login Tests

        [TestMethod]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
            
                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Login", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.AccessToken);
            Assert.NotEmpty(authResponse.AccessToken);
        }

        [TestMethod]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
            
                UserName = "testuser@example.com",
                Password = "WrongPassword@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Login", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
             
                UserName = "nonexistent@example.com",
                Password = "Password@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Login", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticationRequest
            {
           
                UserName = "",
                Password = ""
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Account/Login", request);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                        response.StatusCode == HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Logout Tests

        [TestMethod]
        public async Task Logout_ReturnsOk()
        {
            // Act
            var response = await _client.PostAsync("/Account/Logout", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Logout_AfterLogin_ReturnsOk()
        {
            // Arrange - Login first
            var loginRequest = new AuthenticationRequest
            {
             
                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            await _client.PostAsJsonAsync("/Account/Login", loginRequest);

            // Act
            var response = await _client.PostAsync("/Account/Logout", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region GetAllUsers Tests

        [TestMethod]
        public async Task GetAllUsers_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/Account/GetAllUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetAllUsers_WithAuthentication_ReturnsOkWithUsers()
        {
            // Arrange - Login and get token
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/Account/GetAllUsers");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var users = await response.Content.ReadFromJsonAsync<List<IdentityUser>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [TestMethod]
        public async Task GetAllUsers_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "invalid-token-123");

            // Act
            var response = await _client.GetAsync("/Account/GetAllUsers");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Integration Flow Tests

        [TestMethod]
        public async Task FullUserFlow_RegisterLoginLogout_Success()
        {
            // Step 1: Register
            var registerRequest = new AuthenticationRequest
            {
     
                UserName = "flowtest@example.com",
                Password = "FlowTest@123"
            };

            var registerResponse = await _client.PostAsJsonAsync("/Account/Register", registerRequest);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Step 2: Login
            var loginRequest = new AuthenticationRequest
            {
         
                UserName = "flowtest@example.com",
                Password = "FlowTest@123"
            };

            var loginResponse = await _client.PostAsJsonAsync("/Account/Login", loginRequest);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
            Assert.NotNull(authResponse?.AccessToken);

            // Step 3: Access protected endpoint
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

            var usersResponse = await _client.GetAsync("/Account/GetAllUsers");
            Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);

            // Step 4: Logout
            var logoutResponse = await _client.PostAsync("/Account/Logout", null);
            Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        }

        [TestMethod]
        public async Task MultipleLoginAttempts_WithSameUser_ReturnsNewTokens()
        {
            // Arrange
            var loginRequest = new AuthenticationRequest
            {
               
                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            // Act - Login twice
            var response1 = await _client.PostAsJsonAsync("/Account/Login", loginRequest);
            var authResponse1 = await response1.Content.ReadFromJsonAsync<AuthenticationResponse>();

            var response2 = await _client.PostAsJsonAsync("/Account/Login", loginRequest);
            var authResponse2 = await response2.Content.ReadFromJsonAsync<AuthenticationResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.NotEqual(authResponse1?.AccessToken, authResponse2?.AccessToken);
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public async Task Register_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var response = await _client.PostAsync("/Account/Register", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var response = await _client.PostAsync("/Account/Login", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Register_WithMalformedJson_ReturnsBadRequest()
        {
            // Arrange
            var malformedJson = new StringContent("{invalid json", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Account/Register", malformedJson);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Helper Methods

        [TestMethod]
        private async Task GetAuthTokenAsyncTest()
        {
            var loginRequest = new AuthenticationRequest
            {

                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            var response = await _client.PostAsJsonAsync("/Account/Login", loginRequest);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

   
        private async Task<string> GetAuthTokenAsync()
        {
            var loginRequest = new AuthenticationRequest
            {

                UserName = "testuser@example.com",
                Password = "Test@123456"
            };

            var response = await _client.PostAsJsonAsync("/Account/Login", loginRequest);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            return authResponse?.AccessToken ?? throw new Exception("Failed to get auth token");
        }

        #endregion
    }
}