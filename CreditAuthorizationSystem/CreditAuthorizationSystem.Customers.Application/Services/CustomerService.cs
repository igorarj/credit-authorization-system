using CreditAuthorizationSystem.Customers.Application.Interfaces;
using CreditAuthorizationSystem.Customers.Domain.Models;
using CreditAuthorizationSystem.Customers.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CreditAuthorizationSystem.Customers.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMemoryCache _cache;

        public CustomerService(ICustomerRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _repository.GetByIdAsync(customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            const string cacheKey = "customers_list";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Customer> customers))
            {
                customers = await _repository.GetAllAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _cache.Set(cacheKey, customers, cacheOptions);
            }

            return customers;
        }

        public async Task<Customer> CreateCustomerAsync(string document, string name, decimal creditLimit)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Document = document,
                Name = name,
                CreditLimit = creditLimit
            };
            
            var checkClient = await _repository.GetByDocumentAsync(customer.Document);

            if (checkClient != null)
                return new Customer();

            await _repository.AddAsync(customer);

            return customer;
        }

        public async Task DebitLimiteAsync(Guid customerId, decimal valor)
        {
            var cliente = await _repository.GetByIdAsync(customerId);
            if (cliente == null) return;

            cliente.CreditLimit -= valor;
            await _repository.UpdateAsync(cliente);
        }
    }
}
