using EventBus.Base;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IModel channel;
        private readonly string exchangeType = RabbitMQExchangeTypes.Fanout.ToString().ToLower();

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            ConnectionFactory? connection = config.Connection as ConnectionFactory;

            ArgumentNullException.ThrowIfNull(connection);

            persistentConnection = new RabbitMQPersistentConnection(connection, config.ConnectionRetryCount);

            channel = CreateConsumerChannel();

            SubscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        private void SubscriptionManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            channel.QueueUnbind(queue: eventName,
                                        exchange: EventBusConfig.DefaultTopicName,
                                        routingKey: eventName);

            if (SubscriptionManager.IsEmpty)
            {
                channel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                                .Or<SocketException>()
                                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                                {
                                    // log
                                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            string message = JsonConvert.SerializeObject(@event);
            byte[] body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent



                channel.BasicPublish(
                            exchange: EventBusConfig.DefaultTopicName,
                            routingKey: eventName,
                            mandatory: true,
                            basicProperties: properties,
                            body: body);
            });
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                channel.QueueDeclare(queue: GetSubName(eventName),
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                channel.QueueBind(queue: GetSubName(eventName),
                                            exchange: EventBusConfig.DefaultTopicName,
                                            routingKey: eventName);
            }

            SubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: exchangeType, durable: true, autoDelete: false, arguments: null);

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (channel != null)
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += Consumer_Received;

                channel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            //eventName = "UserCreated"; // core user exchangeinden routing key gelmediği için deneme amaçlı yazıldı.

            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                // log
            }

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        /// <summary>
        /// Bizim üretmediğimiz queueları yakalayabilmek için exchange ismine bağlı olan queueları bulmak için exhchangeName parametrik yapıldı.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="exchangeName"></param>
        public override void Subscribe<T, TH>(string exchangeName)
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                channel.QueueDeclare(queue: GetSubName(eventName),
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                channel.QueueBind(queue: GetSubName(eventName),
                                            exchange: exchangeName,
                                            routingKey: eventName);
            }

            // todo ??? belki AddSubscription methoduna da exchange ismi verilerek routing key yakalamak için kullanılabilir mi bakılacak???
            SubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        /// <summary>
        /// Stock Transaction'ları yönetmek için kullanabiliriz, çünkü stokların stabilliği için tek bir exchange ve queue dinlemek istiyoruz. todo: publish ederken de bu şekilde parametrik kullanmak isteyebiliriz.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        public override void Subscribe<T, TH>(string exchangeName, string queueName)
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                channel.QueueDeclare(queue: queueName,
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                channel.QueueBind(queue: queueName,
                                            exchange: exchangeName,
                                            routingKey: eventName);
            }

            // todo ??? belki AddSubscription methoduna da exchange ismi verilerek routing key yakalamak için kullanılabilir mi bakılacak???
            SubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void Publish(IntegrationEvent @event, string exchangeName)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                                .Or<SocketException>()
                                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                                {
                                    // log
                                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            string message = JsonConvert.SerializeObject(@event);
            byte[] body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

       

                channel.BasicPublish(
                            exchange: exchangeName,
                            routingKey: eventName,
                            mandatory: true,
                            basicProperties: properties,
                            body: body);
            });
        }
    }
}
