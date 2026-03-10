using CreditAuthorizationSystem.Customers.Domain.Models;
using CreditAuthorizationSystem.Customers.Domain.Repositories;

namespace CreditAuthorizationSystem.Customers.Infrastructure.Repositories
{
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private static readonly List<Customer> Customers = new();

        public async Task<Customer> AddAsync(Customer customer)
        {
            Customers.Add(customer);

            return customer;
        }

        public async Task<Customer> GetByDocumentAsync(string document)
        {
            var result = Customers.FirstOrDefault(c => c.Document == document);

            return result;
        }

        public async Task<Customer> GetByIdAsync(Guid customerId)
        {
            var result = Customers.FirstOrDefault(c => c.Id == customerId);

            return result;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existing = Customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Cliente com Id {customer.Id} não encontrado.");

            Customers.Remove(existing);
            Customers.Add(customer);

            return customer;
        }
    }
}
