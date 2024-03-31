using EventBus.Base;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    public class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            return config.EventBusType switch
            {
                EventBusTypes.RabbitMQ => new EventBusRabbitMQ(config, serviceProvider),
                EventBusTypes.AzureServiceBus => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
