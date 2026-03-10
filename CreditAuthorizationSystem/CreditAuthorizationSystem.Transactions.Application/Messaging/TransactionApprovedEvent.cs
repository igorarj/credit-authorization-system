namespace CreditAuthorizationSystem.Transactions.Application.Messaging
{
    public class TransactionApprovedEvent
    {
        public Guid IdCliente { get; set; }
        public decimal ValorDebitado { get; set; }
        public Guid IdTransacao { get; set; }
    }
}
