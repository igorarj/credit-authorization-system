using CreditAuthorizationSystem.Customers.Domain.Models;

namespace CreditAuthorizationSystem.Customers.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByDocumentAsync(string document);
        Task<Customer> GetByIdAsync(Guid customerId);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
    }
}
