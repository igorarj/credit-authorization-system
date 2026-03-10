namespace CreditAuthorizationSystem.Customers.Api.Contracts
{
    public class CreateCustomerErrorResponse : BaseResponse
    {
        public string detalheErro { get; set; }
        public CreateCustomerErrorResponse(string status, string detalheErro)
        {
            this.status = status;
            this.detalheErro = detalheErro;
        }
    }
}
