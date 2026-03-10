using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using CreditAuthorizationSystem.Transactions.Application.Messaging;
using CreditAuthorizationSystem.Transactions.Domain.Models;
using CreditAuthorizationSystem.Transactions.Domain.Repositories;

namespace CreditAuthorizationSystem.Transactions.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly ITransactionEventPublisher _publisher;
        private readonly ICustomerApiClient _customerClient;

        public TransactionService(
            ITransactionRepository repository,
            ITransactionEventPublisher publisher,
            ICustomerApiClient customerClient)
        {
            _repository = repository;
            _publisher = publisher;
            _customerClient = customerClient;
        }

        public async Task<Transaction> CreateTransactionAsync(Guid idCliente, decimal amount)
        {
            var customer = await _customerClient.GetCustomerByIdAsync(idCliente);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                CustomerId = idCliente,
                Approved = true,
                Amount = amount
            };

            if (customer == null)
            {
                transaction.CustomerId = Guid.Empty;
                transaction.Approved = false;
            }
            else if (customer.CreditLimit < amount)
            {
                transaction.Amount = -1;
                transaction.Approved = false;
            }

            if (transaction.Approved)
            {
                transaction.CustomerId = customer.Id;
                transaction.Approved = true;
                transaction.Amount = amount;
                await _repository.AddAsync(transaction);
                var evt = new TransactionApprovedEvent
                {
                    IdCliente = transaction.CustomerId,
                    ValorDebitado = transaction.Amount,
                    IdTransacao = transaction.Id
                };

                await _publisher.PublishTransactionApprovedAsync(evt); 
            }

            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid idCliente)
        {
            return await _repository.GetByCustomerAsync(idCliente);
        }
    }
}
