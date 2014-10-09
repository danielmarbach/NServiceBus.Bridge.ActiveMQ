using System;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace NServiceBus.Bridge.ActiveMQ2.Msmq
{
    public class MsmqToActiveMqMessageConverter
    {
        private readonly string replyToQueue;

        private readonly CorrelationIdAndAddressTranscoder correlationIdAndAddressTranscoder;

        public MsmqToActiveMqMessageConverter(
            CorrelationIdAndAddressTranscoder correlationIdAndAddressTranscoder,
            string replyToQueue)
        {
            this.replyToQueue = replyToQueue;
            this.correlationIdAndAddressTranscoder = correlationIdAndAddressTranscoder;
        }

        public ITextMessage CreateActiveMqTextMessageFromMsmqTransportMessage(
            TransportMessage transportMessage,
            string messageType,
            Func<string> defaultQueueCallback)
        {
            messageType = messageType.Split(',')[0];
            var body = Encoding.UTF8.GetString(transportMessage.Body);
            string receivedCorrelationId = transportMessage.CorrelationId ?? transportMessage.Id;
            CorrelationIdAndAddress correlationIdAndAddress = this.correlationIdAndAddressTranscoder.DecodeCorrelationId(receivedCorrelationId);
            string address = correlationIdAndAddress.Address ?? defaultQueueCallback();
            var destination = transportMessage.MessageIntent == MessageIntentEnum.Publish
                                  ? (IDestination)new ActiveMQTopic("VirtualTopic." + messageType)
                                  : (IDestination)new ActiveMQQueue(address);

            ITextMessage amqMessage = new ActiveMQTextMessage(body);

            amqMessage.NMSCorrelationID = this.correlationIdAndAddressTranscoder.EncodeAddressToCorrelationId(receivedCorrelationId, correlationIdAndAddress.CorrelationId, TryToGetReplyToQueueName(transportMessage));
            amqMessage.NMSType = messageType;
            amqMessage.NMSReplyTo = new ActiveMQQueue(this.replyToQueue);
            amqMessage.NMSDestination = destination;

            return amqMessage;
        }

        private static string TryToGetReplyToQueueName(TransportMessage transportMessage)
        {
            if (transportMessage.ReplyToAddress == null)
            {
                return string.Empty;
            }

            return transportMessage.ReplyToAddress.ToString();
        }
    }
}