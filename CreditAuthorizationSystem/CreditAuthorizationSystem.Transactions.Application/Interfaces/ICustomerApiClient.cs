using CreditAuthorizationSystem.Transactions.Application.DTOs;

namespace CreditAuthorizationSystem.Transactions.Application.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
    }
}
