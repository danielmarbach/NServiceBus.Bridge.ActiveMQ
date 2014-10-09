using System;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class MessageSender
    {
        private readonly IBus bus;

        private readonly AddressDirectory addressDirectory;

        public MessageSender(IBus bus, AddressDirectory addressDirectory)
        {
            this.bus = bus;
            this.addressDirectory = addressDirectory;
        }

        public virtual void SendMessage(
            object message,
            CorrelationIdAndAddress correlationIdAndAddress,
            Type type)  
        {
            Headers.SetMessageHeader(message, Headers.CorrelationId, correlationIdAndAddress.CorrelationId);

            ////if (!Conventions.Messages(type))
            ////{
            ////    Headers.SetMessageHeader(message, Headers.MessageId, correlationIdAndAddress.CorrelationId);
            ////}

            ////if (Conventions.Events(type))
            ////{
            ////    this.bus.Publish(message);
            ////}
            ////else
            ////{
            ////    var address = !string.IsNullOrEmpty(correlationIdAndAddress.Address)
            ////        ? Address.Parse(correlationIdAndAddress.Address)
            ////        : this.addressDirectory.GetAddress(type);

            ////    this.bus.Send(address, message);
            ////}
        }
    }
}