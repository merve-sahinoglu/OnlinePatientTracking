using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Publish(IntegrationEvent @event, string exchangeName);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void Subscribe<T, TH>(string exchangeName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void Subscribe<T, TH>(string exchangeName, string queueName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
