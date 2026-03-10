namespace CreditAuthorizationSystem.Transactions.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public decimal Amount { get; set; }

        public bool Approved { get; set; }
    }
}
