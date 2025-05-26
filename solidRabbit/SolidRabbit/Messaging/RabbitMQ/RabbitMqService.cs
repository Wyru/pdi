using SolidRabbit.Messaging.Contracts;
using SolidRabbit.Messaging.Models;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SolidRabbit.Messaging.RabbitMQ
{
    public class RabbitMqService : IMessagingService
    {
        private readonly ConnectionFactory _factory;
        private IConnection connection;
        private IChannel channel;
        private bool _disposed = false;

        public RabbitMqService(RabbitMqConnectionSettings settings) {
            _factory = new ConnectionFactory() {
                HostName = settings.HostName,
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password
            };
            Connect();
        }

        private async Task Connect() {
            if (connection == null || !connection.IsOpen) {
                try {
                    connection = await _factory.CreateConnectionAsync();
                    channel = await connection.CreateChannelAsync();
                }
                catch (Exception ex) {
                    // Lógica de retry pode ser adicionada aqui
                    throw;
                }
            }
        }

        public async Task PublishAsync<T>(string exchangeName, string routingKey, T message) {

            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqService));

            if (channel == null || !channel.IsOpen) await Connect();

            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: "topic", durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(exchangeName, routingKey, true, body);
        }

        public async Task SubscribeAsync<T>(string queueName, string exchangeName, string routingKey, Action<T> handler) {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqService));

            if (channel == null || !channel.IsOpen) await Connect();

            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: "topic", durable: true);
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: true, autoDelete: true, arguments: null);
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) => {
                var body = ea.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);
                
                var message = JsonSerializer.Deserialize<T>(messageString);
                handler?.Invoke(message);

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void StartConsuming() {
            // Consumo já começa com BasicConsume
        }

        public void StopConsuming() {
            // Opcional: Para parar o consumo, você precisaria manter referências aos consumers.
            // Para este caso, o Dispose cuidará de fechar o canal.
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;

            if (disposing) {
                channel?.CloseAsync();
                channel?.Dispose();
                connection?.CloseAsync();
                connection?.Dispose();
            }
            _disposed = true;
        }
    }
}
