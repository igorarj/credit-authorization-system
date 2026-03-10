using CreditAuthorizationSystem.Customers.Domain.Models;

namespace CreditAuthorizationSystem.Customers.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerAsync(Guid customerId);
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer> CreateCustomerAsync(string document, string name, decimal creditLimit);
        Task DebitLimiteAsync(Guid customerId, decimal valor);
    }
}
