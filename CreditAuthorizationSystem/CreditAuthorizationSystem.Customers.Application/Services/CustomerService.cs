using CreditAuthorizationSystem.Customers.Application.Interfaces;
using CreditAuthorizationSystem.Customers.Domain.Models;
using CreditAuthorizationSystem.Customers.Domain.Repositories;

namespace CreditAuthorizationSystem.Customers.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _repository.GetByIdAsync(customerId);
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
