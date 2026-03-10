namespace CreditAuthorizationSystem.Transactions.Api.Contracts
{
    public class CreateTransactionRequest
    {
        public Guid CustomerId { get; set; }

        public decimal Amount { get; set; }
    }
}
