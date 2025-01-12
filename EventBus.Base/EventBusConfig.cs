﻿

namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;
        public string DefaultTopicName { get; set; } = "mm";
        public string EventBusConnectionString { get; set; } = String.Empty;

 
        public string SubscriberClientAppName { get; set; } = String.Empty;
        public string EventNamePrefix { get; set; } = String.Empty;
        public string EventNameSuffix { get; set; } = "Event";
        public EventBusTypes EventBusType { get; set; } = EventBusTypes.RabbitMQ;


        public object Connection { get; set; }

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);
        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);
    }

    public enum EventBusTypes
    {
        RabbitMQ = 0,
        AzureServiceBus = 1
    }
}
