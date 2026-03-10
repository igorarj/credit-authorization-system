namespace CreditAuthorizationSystem.Customers.Api.Contracts
{
    public class CreateCustomerRequest
    {
        public string Document { get; set; }
        public string Name { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
