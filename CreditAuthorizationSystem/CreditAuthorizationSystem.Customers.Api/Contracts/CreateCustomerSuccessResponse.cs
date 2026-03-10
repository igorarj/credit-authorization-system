namespace CreditAuthorizationSystem.Customers.Api.Contracts
{
    public class CreateCustomerSuccessResponse : BaseResponse
    {
        public Guid idCliente { get; set; }

        public CreateCustomerSuccessResponse(Guid idCliente, string status)
        {
            this.status = status;
            this.idCliente = idCliente;
        }
    }
}
