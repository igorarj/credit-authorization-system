using CreditAuthorizationSystem.Transactions.Domain.Models;

namespace CreditAuthorizationSystem.Transactions.Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction transaction);

        Task<IEnumerable<Transaction>> GetByCustomerAsync(Guid customerId);
    }
}
