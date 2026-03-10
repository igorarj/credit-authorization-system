namespace CreditAuthorizationSystem.Transactions.Application.Messaging
{
    public interface ITransactionEventPublisher
    {
        Task PublishTransactionApprovedAsync(TransactionApprovedEvent evt);
    }
}
