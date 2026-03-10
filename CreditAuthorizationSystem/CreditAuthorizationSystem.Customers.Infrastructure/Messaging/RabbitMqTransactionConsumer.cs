using CreditAuthorizationSystem.Customers.Application.Messaging;
using CreditAuthorizationSystem.Customers.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CreditAuthorizationSystem.Customers.Infrastructure.Messaging
{
    public class RabbitMqTransactionConsumer : BackgroundService
    {
        private readonly ICustomerRepository _repository;
        private IChannel _channel;
        private IConnection _connection;
        private const string QueueName = "transaction-approved";
        private string _hostName;
        private string _userName;
        private string _password;

        public RabbitMqTransactionConsumer(ICustomerRepository repository, string hostName, string userName, string password)
        {
            _repository = repository;
            _hostName = hostName;
            _userName = userName;
            _password = password;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();

                    await _channel.QueueDeclareAsync(
                        queue: QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    break;
                }
                catch
                {
                    Console.WriteLine("RabbitMQ ainda não está pronto, tentando novamente em 2s...");
                    await Task.Delay(2000, stoppingToken);
                }
            }

            await StartConsumingAsync(stoppingToken);
        }

            private Task StartConsumingAsync(CancellationToken stoppingToken)
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, args) =>
                {
                    bool shouldAck = true;
                    try
                    {
                        var body = args.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var evt = JsonSerializer.Deserialize<TransactionApprovedEvent>(json);

                        if (evt == null)
                        {
                            Console.WriteLine("Mensagem inválida");
                            return;
                        }

                        var customer = await _repository.GetByIdAsync(evt.IdCliente);

                        if (customer == null)
                        {
                            Console.WriteLine($"Cliente {evt.IdCliente} não encontrado");
                            return;
                        }

                        if (customer.CreditLimit < evt.ValorDebitado)
                        {
                            Console.WriteLine($"Cliente {evt.IdCliente} não possui saldo suficiente (R${customer.CreditLimit}/R${evt.ValorDebitado})");
                            return;
                        }

                        customer.CreditLimit -= evt.ValorDebitado;
                        await _repository.UpdateAsync(customer);
                        Console.WriteLine($"Limite do cliente {customer.Name} atualizado: {customer.CreditLimit}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                        shouldAck = false;
                    }
                    finally
                    {
                        if (shouldAck)
                        {
                            await _channel.BasicAckAsync(args.DeliveryTag, false);
                        }
                    }
                };

                _channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                return Task.CompletedTask;
            }
    }
}
