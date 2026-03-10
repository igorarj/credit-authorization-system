using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace CreditAuthorizationSystem.Transactions.Infrastructure.HttpClients
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _http;
        private readonly string _userName;
        private readonly string _password;  

        public AuthApiClient(HttpClient http, string userName, string password)
        {
            _http = http;
            _userName = userName;
            _password = password;
        }

        public async Task<string> GetTokenAsync()
        {
            Console.WriteLine($"user: {_userName} pass: {_password} base: {_http.BaseAddress}");

            var request = new AuthLoginRequest(_userName, _password);
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            Console.WriteLine($"response: {response}");

            if (!response.IsSuccessStatusCode) return null;

            var jsonString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonString);
            if (doc.RootElement.TryGetProperty("token", out var tokenElement))
            {
                return tokenElement.GetString();
            }

            return null;
        }

        private class AuthLoginRequest
        {
            public string email { get; set; }
            public string password { get; set; }
            public AuthLoginRequest(string userName, string password)
            {
                this.email = userName;
                this.password = password;
            }
        }
    }
}
