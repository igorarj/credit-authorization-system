
using CreditAuthorizationSystem.Transactions.Domain.Models;

namespace CreditAuthorizationSystem.Transactions.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(Guid customerId, decimal amount);

        Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid customerId);
    }
}
