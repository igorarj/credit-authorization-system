using CreditAuthorizationSystem.Transactions.Api.Contracts;
using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CreditAuthorizationSystem.Transactions.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
        {
            var transaction = await _service.CreateTransactionAsync(request.CustomerId, request.Amount);

            if (transaction.CustomerId == null || transaction.CustomerId == Guid.Empty)
                return Ok(new CreateTransactionErrorResponse("NEGADA", $"O cliente {request.CustomerId} não foi encontrado."));

            if (transaction.Amount < 0)
                return Ok(new CreateTransactionErrorResponse("NEGADA", $"O cliente não possui saldo suficiente."));

            return Ok(new CreateTransactionSuccessResponse("APROVADA", transaction.Id));
        }

        [HttpGet("{clienteId}")]
        public async Task<IActionResult> GetTransactions(Guid clienteId)
        {
            var transactions = await _service.GetTransactionsAsync(clienteId);

            return Ok(transactions);
        }
    }
}
