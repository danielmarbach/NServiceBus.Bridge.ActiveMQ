using System;
using System.Globalization;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class ActiveMqExceptionHandler
    {
        private readonly ActiveMqMessageHandler messageHandler;

        public ActiveMqExceptionHandler(ActiveMqMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public void ProcessMessage(Apache.NMS.IMessage amqMessage, IMessageProducer errorMessageProducer)
        {
            try
            {
                var amqTextMessage = (ITextMessage)amqMessage;
                this.messageHandler.ProcessMessage(amqTextMessage);
            }
            catch (Exception e)
            {
                ActiveMQMessage errorMessage = (ActiveMQMessage)((ActiveMQMessage)amqMessage).Clone();
                errorMessage.ReadOnlyProperties = false;
                errorMessage.Properties["Bridge.Error.Message"] = e.ToString();
                errorMessage.Properties["Bridge.Error.OriginalDestination"] = amqMessage.NMSDestination.ToString();
                errorMessage.Properties["Bridge.Error.OriginalTimestamp"] = amqMessage.NMSTimestamp.ToString(CultureInfo.InvariantCulture);
                errorMessage.Properties["Bridge.Error.OriginalMessageId"] = amqMessage.NMSMessageId;

                errorMessageProducer.Send(errorMessage);
            }
        }
    }
}