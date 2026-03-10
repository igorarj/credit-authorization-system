using CreditAuthorizationSystem.Transactions.Domain.Models;
using CreditAuthorizationSystem.Transactions.Domain.Repositories;

namespace CreditAuthorizationSystem.Transactions.Infrastructure.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private static readonly List<Transaction> Transactions = new();

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            Transactions.Add(transaction);
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetByCustomerAsync(Guid customerId)
        {
            return Transactions.Where(t => t.CustomerId == customerId);
        }
    }
}
