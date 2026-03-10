using Microsoft.AspNetCore.Mvc;
using CreditAuthorizationSystem.Customers.Application.Interfaces;
using CreditAuthorizationSystem.Customers.Api.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace CreditAuthorizationSystem.Customers.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            var result = await _service.CreateCustomerAsync(request.Document, request.Name, request.CreditLimit);

            if (result.Id == null || result.Id == Guid.Empty)
                return BadRequest(new CreateCustomerErrorResponse("ERRO", $"Já existe um cliente com o documento {request.Document} cadastrado."));

            return Ok(new CreateCustomerSuccessResponse(result.Id, "SUCESSO"));
        }

        [HttpGet("{clienteId}")]
        public async Task<IActionResult> GetCustomer(Guid clienteId)
        {
            var customer = await _service.GetCustomerAsync(clienteId);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _service.GetCustomersAsync();
            return Ok(customers);
        }
    }
}
