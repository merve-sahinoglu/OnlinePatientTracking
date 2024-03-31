namespace EventBus.Base
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        /// <summary>
        /// Dışarıdan gelen unsubscribe methodu çalıştığında bu event call tetiklenecek.
        /// </summary>
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void RemoveSubscription<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;

        /// <summary>
        /// Tipi verilen eventi dinleyip dinlemediğimizi kontrol eden method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        /// <summary>
        /// Adı verilen eventi dinleyip dinlemediğimizi kontrol eden method.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);

        /// <summary>
        /// Event name parametresi alıp o eventin handlerını dönecek.
        /// Örneğin : ItemCreatedEvent => ItemCreatedIntegrationHandler tipini dönecek.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        /// <summary>
        /// Örneğin RabbitMQ'daki RoutingKey bilgisini dönecek
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetEventKey<T>();
    }
}
