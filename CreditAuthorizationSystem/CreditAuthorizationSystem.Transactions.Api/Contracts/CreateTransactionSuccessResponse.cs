using System.Diagnostics;

namespace CreditAuthorizationSystem.Transactions.Api.Contracts
{
    public class CreateTransactionSuccessResponse : BaseResponse
    {
        public Guid idTransacao { get; set; }

        public CreateTransactionSuccessResponse(string status, Guid idTransacao)
        {
            this.status = status;
            this.idTransacao = idTransacao;
        }
    }
}
