using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base
{
    public abstract class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;

        public EventBusConfig EventBusConfig { get; set; }

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config;
            ServiceProvider = serviceProvider;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        public virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());

            if (EventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());

            return eventName;
        }

        public virtual string GetSubName(string eventName)
        {
            string subscriptionName = $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
            return subscriptionName;
        }

        public virtual void Dispose()
        {
            EventBusConfig = null;
        }

        /// <summary>
        /// RabbitMQ'ya iletilen mesajlar, oradan buraya gelecek. RabbitMQ buraya yollayacak.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            try
            {
                eventName = ProcessEventName(eventName);

                var processed = false;

                if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
                {
                    /// event'e (Örneğin ItemUpdatedEvent) subscribe olan bütün subscriptionları döner. (Çünkü item'ı warehouse şeması ve purchase şeması dinleyecek)
                    var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);

                    using (var scope = ServiceProvider.CreateScope())
                    {
                        foreach (var subscription in subscriptions)
                        {
                            var handler = ServiceProvider.GetService(subscription.HandlerType);

                            if (handler == null) continue;

                            var eventType = SubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");

                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }

                    processed = true;
                }

                return processed;
            }
            catch (Exception ex)
            {
                // log ex
                return false;
            }
        }

        public abstract void Publish(IntegrationEvent @event);
        public abstract void Publish(IntegrationEvent @event, string exchangeName);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public abstract void Subscribe<T, TH>(string exchangeName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public abstract void Subscribe<T, TH>(string exchangeName, string queueName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
