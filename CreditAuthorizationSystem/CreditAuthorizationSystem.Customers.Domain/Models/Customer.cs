namespace CreditAuthorizationSystem.Customers.Domain.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Document { get; set; }
        public string Name { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
