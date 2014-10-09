using System;
using Apache.NMS;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class ActiveMqMessageHandler
    {
        private readonly CorrelationIdAndAddressTranscoder correlationIdAndAddressTranscoder;

        private readonly ActiveMqMessageDeserializer messageDeserializer;

        private readonly MessageSender messageSender;

        private readonly MessageTypeInterpreter messageTypeInterpreter;

        public ActiveMqMessageHandler(
            CorrelationIdAndAddressTranscoder correlationIdAndAddressTranscoder,
            ActiveMqMessageDeserializer messageDeserializer,
            MessageSender messageSender,
            MessageTypeInterpreter messageTypeInterpreter)
        {
            this.correlationIdAndAddressTranscoder = correlationIdAndAddressTranscoder;
            this.messageDeserializer = messageDeserializer;
            this.messageSender = messageSender;
            this.messageTypeInterpreter = messageTypeInterpreter;
        }

        public virtual void ProcessMessage(ITextMessage amqMessage)
        {
            Type type = this.messageTypeInterpreter.GetTypeFromNmsType(amqMessage.NMSType);
            object message = this.messageDeserializer.DeserializeMessage(amqMessage, type);

            CorrelationIdAndAddress correlationIdAndAddress =
                this.correlationIdAndAddressTranscoder.TranscodeCorrelationIdAndAddress(
                    amqMessage.NMSCorrelationID ?? amqMessage.NMSMessageId,
                    TryToGetReplyToQueueName(amqMessage));
                      
            this.messageSender.SendMessage(
                message,
                correlationIdAndAddress,
                type);
        }

        private static string TryToGetReplyToQueueName(ITextMessage amqMessage)
        {
            if (amqMessage.NMSReplyTo == null)
            {
                return string.Empty;
            }
            
            return ((IQueue)amqMessage.NMSReplyTo).QueueName;
        }
    }
}