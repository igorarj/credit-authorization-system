using CreditAuthorizationSystem.Transactions.Application.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CreditAuthorizationSystem.Transactions.Infrastructure.Messaging
{
    public class RabbitMqTransactionEventPublisher : ITransactionEventPublisher, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private const string QueueName = "transaction-approved";

        public RabbitMqTransactionEventPublisher(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();
        }

        public async Task PublishTransactionApprovedAsync(TransactionApprovedEvent evt)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                body: body
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.DisposeAsync();
            if (_connection != null) await _connection.DisposeAsync();
        }
    }
}
