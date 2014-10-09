using System.Linq;
using Apache.NMS;
using NServiceBus.Scheduling.Messages;
using NServiceBus.Unicast;

namespace NServiceBus.Bridge.ActiveMQ2.Msmq
{
    public class MsmqMessageHandler : IHandleMessages<object>
    {
        private readonly MsmqToActiveMqMessageConverter messageConverter;

        private readonly IConnectionFactory connectionFactory;

        private readonly IBus bus;

        public MsmqMessageHandler(IBus bus, IConnectionFactory connectionFactory, MsmqToActiveMqMessageConverter messageConverter)
        {
            this.bus = bus;
            this.connectionFactory = connectionFactory;
            this.messageConverter = messageConverter;
        }

        public void Handle(object message)
        {
            if (message is ScheduledTask)
            {
                return;
            }
            
            var transportMessage = this.bus.CurrentMessageContext.GetTransportMessage();
            
            var type = this.bus.CurrentMessageContext.Headers[Headers.EnclosedMessageTypes];
            var amqMessage = this.messageConverter.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                type,
                () => this.GetQueue(message));

            ////if (this.bus.CurrentMessageContext.Headers.ContainsKey(Constants.SpecialHeader))
            ////{
            ////    var specialHeader = this.bus.CurrentMessageContext.Headers[Constants.SpecialHeader];
            ////    amqMessage.Properties.SetString(Constants.SpecialHeader, specialHeader);
            ////}

            using (var connection = this.connectionFactory.CreateConnection())
            using (var session = connection.CreateSession())
            using (var producer = session.CreateProducer())
            {
                producer.Send(amqMessage.NMSDestination, amqMessage);
            }
        }

        private string GetQueue(object message)
        {
            return ((UnicastBus)this.bus).GetAddressForMessageType(message.GetType()).Single().Queue;
        }
    }
}