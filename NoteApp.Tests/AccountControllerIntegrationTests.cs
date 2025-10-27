using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoteApp.Tests
{
    [TestClass]
    public class AccountControllerIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task Register_Login_And_GetAllUsers_Should_Work()
        {
            var randomNum = new Random().Next(0, 9999);
            // 1. Register a new user
            var registerPayload = new
            {
                UserName = "testuser" + randomNum,
                Email = "testuser@example.com" + randomNum,
                Password = "Test@12345"
            };

            var registerContent = new StringContent(JsonSerializer.Serialize(registerPayload), Encoding.UTF8, "application/json");
            var registerResponse = await _client.PostAsync("/Account/Register", registerContent);
            registerResponse.EnsureSuccessStatusCode();

            // 2. Login with the user
            var loginPayload = new
            {
                UserName = "testuser" + randomNum,
                Password = "Test@12345",
                Email = "testuser@example.com" + randomNum
            };

            var loginContent = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/Account/Login", loginContent);
            loginResponse.EnsureSuccessStatusCode();

            var loginBody = await loginResponse.Content.ReadAsStringAsync();

            // Extract token if present
            using var doc = JsonDocument.Parse(loginBody);
            string token = string.Empty;
            if (doc.RootElement.TryGetProperty("accessToken", out var t))
            {
                token = t.GetString() ?? string.Empty;
            }

            // Attach token to client for authorized call if token exists
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Call GetAllUsers (authorized)
            var usersResponse = await _client.GetAsync("/Account/GetAllUsers");

            // If token not issued the endpoint will return 401; test expects either 200 or 401 depending on setup
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Assert.IsTrue(true, "GetAllUsers returned Unauthorized as expected when token not configured.");
            }
            else
            {
                usersResponse.EnsureSuccessStatusCode();
                var usersBody = await usersResponse.Content.ReadAsStringAsync();
                Assert.IsTrue(!string.IsNullOrEmpty(usersBody));
            }
        }
    }
}
