using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidRabbit.Messaging.Contracts
{
    public interface IMessagingService : IDisposable
    {
        Task PublishAsync<T>(string exchangeName, string routingKey, T message);
        Task SubscribeAsync<T>(string queueName, string exchangeName, string routingKey, Action<T> handler);
        void StartConsuming();
        void StopConsuming();
    }
}
