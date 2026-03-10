using CreditAuthorizationSystem.Transactions.Application.DTOs;
using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using CreditAuthorizationSystem.Transactions.Application.Messaging;
using CreditAuthorizationSystem.Transactions.Application.Services;
using CreditAuthorizationSystem.Transactions.Domain.Models;
using CreditAuthorizationSystem.Transactions.Domain.Repositories;
using Moq;

namespace CreditAuthorizationSystem.Transactions.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repositoryMock;
        private readonly Mock<ITransactionEventPublisher> _publisherMock;
        private readonly Mock<ICustomerApiClient> _customerClientMock;

        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _repositoryMock = new Mock<ITransactionRepository>();
            _publisherMock = new Mock<ITransactionEventPublisher>();
            _customerClientMock = new Mock<ICustomerApiClient>();

            _service = new TransactionService(
                _repositoryMock.Object,
                _publisherMock.Object,
                _customerClientMock.Object
            );
        }

        [Fact]
        public async Task CreateTransaction_ShouldApprove_WhenCustomerHasCredit()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 1000
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            var result = await _service.CreateTransactionAsync(customerId, 100);

            Assert.True(result.Approved);
            Assert.Equal(100, result.Amount);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Transaction>()), Times.Once);
            _publisherMock.Verify(p => p.PublishTransactionApprovedAsync(It.IsAny<TransactionApprovedEvent>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReject_WhenCustomerDoesNotExist()
        {
            var customerId = Guid.NewGuid();

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync((CustomerDto?)null);

            var result = await _service.CreateTransactionAsync(customerId, 100);

            Assert.False(result.Approved);
            Assert.Equal(Guid.Empty, result.CustomerId);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Transaction>()), Times.Never);
            _publisherMock.Verify(p => p.PublishTransactionApprovedAsync(It.IsAny<TransactionApprovedEvent>()), Times.Never);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReject_WhenCreditLimitIsInsufficient()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 50
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            var result = await _service.CreateTransactionAsync(customerId, 100);

            Assert.False(result.Approved);
            Assert.Equal(-1, result.Amount);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Transaction>()), Times.Never);
            _publisherMock.Verify(p => p.PublishTransactionApprovedAsync(It.IsAny<TransactionApprovedEvent>()), Times.Never);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnTransactionsFromRepository()
        {
            var customerId = Guid.NewGuid();

            var transactions = new List<Domain.Models.Transaction>
            {
                new Domain.Models.Transaction { Id = Guid.NewGuid(), CustomerId = customerId, Amount = 100, Approved = true }
            };

            _repositoryMock
                .Setup(r => r.GetByCustomerAsync(customerId))
                .ReturnsAsync(transactions);

            var result = await _service.GetTransactionsAsync(customerId);

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateTransaction_ShouldApprove_WhenAmountEqualsCreditLimit()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 100
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            var result = await _service.CreateTransactionAsync(customerId, 100);

            Assert.True(result.Approved);
            Assert.Equal(100, result.Amount);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_ShouldPublishEvent_WithCorrectValues()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 1000
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            await _service.CreateTransactionAsync(customerId, 200);

            _publisherMock.Verify(p =>
                p.PublishTransactionApprovedAsync(
                    It.Is<TransactionApprovedEvent>(evt =>
                        evt.IdCliente == customerId &&
                        evt.ValorDebitado == 200
                    )
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateTransaction_ShouldSaveTransaction_WithCorrectValues()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 1000
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            await _service.CreateTransactionAsync(customerId, 300);

            _repositoryMock.Verify(r =>
                r.AddAsync(It.Is<Transaction>(t =>
                    t.CustomerId == customerId &&
                    t.Amount == 300 &&
                    t.Approved == true
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateTransaction_ShouldNotPublishEvent_WhenRejected()
        {
            var customerId = Guid.NewGuid();

            var customer = new CustomerDto
            {
                Id = customerId,
                CreditLimit = 10
            };

            _customerClientMock
                .Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            await _service.CreateTransactionAsync(customerId, 100);

            _publisherMock.Verify(
                p => p.PublishTransactionApprovedAsync(It.IsAny<TransactionApprovedEvent>()),
                Times.Never
            );
        }
    }
}