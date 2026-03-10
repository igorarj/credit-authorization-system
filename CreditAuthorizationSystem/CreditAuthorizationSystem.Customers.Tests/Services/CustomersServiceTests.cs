using CreditAuthorizationSystem.Customers.Application.Services;
using CreditAuthorizationSystem.Customers.Domain.Models;
using CreditAuthorizationSystem.Customers.Domain.Repositories;
using Moq;

namespace CreditAuthorizationSystem.Customers.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _service = new CustomerService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetCustomerAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            var id = Guid.NewGuid();

            var customer = new Customer
            {
                Id = id,
                Document = "123",
                Name = "Igor",
                CreditLimit = 1000
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(customer);

            var result = await _service.GetCustomerAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldCreateCustomer_WhenDocumentDoesNotExist()
        {
            var document = "123";
            var name = "Igor";
            var limit = 1000;

            _repositoryMock
                .Setup(r => r.GetByDocumentAsync(document))
                .ReturnsAsync((Customer)null);

            var result = await _service.CreateCustomerAsync(document, name, limit);

            Assert.Equal(document, result.Document);
            Assert.Equal(name, result.Name);
            Assert.Equal(limit, result.CreditLimit);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldNotCreateCustomer_WhenDocumentAlreadyExists()
        {
            var document = "123";

            var existingCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                Document = document,
                Name = "Existing",
                CreditLimit = 1000
            };

            _repositoryMock
                .Setup(r => r.GetByDocumentAsync(document))
                .ReturnsAsync(existingCustomer);

            var result = await _service.CreateCustomerAsync(document, "Igor", 1000);

            Assert.Equal(Guid.Empty, result.Id);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task DebitLimiteAsync_ShouldDebitCredit_WhenCustomerExists()
        {
            var id = Guid.NewGuid();

            var customer = new Customer
            {
                Id = id,
                Document = "123",
                Name = "Igor",
                CreditLimit = 1000
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(customer);

            await _service.DebitLimiteAsync(id, 200);

            Assert.Equal(800, customer.CreditLimit);

            _repositoryMock.Verify(r => r.UpdateAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DebitLimiteAsync_ShouldDoNothing_WhenCustomerDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Customer)null);

            await _service.DebitLimiteAsync(id, 200);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }
    }
}