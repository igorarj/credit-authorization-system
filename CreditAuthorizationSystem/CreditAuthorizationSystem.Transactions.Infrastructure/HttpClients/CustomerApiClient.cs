using CreditAuthorizationSystem.Transactions.Application.DTOs;
using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using System.Text.Json;

namespace CreditAuthorizationSystem.Transactions.Infrastructure.HttpClients
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _http;
        private readonly IAuthApiClient _authApiClient;

        public CustomerApiClient(HttpClient http, IAuthApiClient authApiClient)
        {
            _http = http;
            _authApiClient = authApiClient;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            Console.WriteLine($"id: {id}");

            var token = await _authApiClient.GetTokenAsync();

            Console.WriteLine($"token: {token}");

            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync($"/api/customers/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
