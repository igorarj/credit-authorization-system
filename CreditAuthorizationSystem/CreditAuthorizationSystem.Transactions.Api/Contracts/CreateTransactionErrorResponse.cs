namespace CreditAuthorizationSystem.Transactions.Api.Contracts
{
    public class CreateTransactionErrorResponse : BaseResponse
    {
        public string detalheErro { get; set; }

        public CreateTransactionErrorResponse(string status, string detalheErro)
        {
            this.status = status;
            this.detalheErro = detalheErro;
        }
    }
}
