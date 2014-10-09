using System.Text;
using Apache.NMS;
using FluentAssertions;
using NServiceBus.Bridge.ActiveMQ2.Msmq;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test.Msmq
{
    [TestFixture]
    public class MsmqToActiveMqMessageConverterTest
    {
        private const string ActiveMqReplyToQueue = "queue://amqReplyToQueue";

        private MsmqToActiveMqMessageConverter testee;

        [SetUp]
        public void SetUp()
        {
            this.testee = new MsmqToActiveMqMessageConverter(new CorrelationIdAndAddressTranscoder(), ActiveMqReplyToQueue);
        }

        [Test]
        public void Convert_CopiesBodyAndMessageTypes()
        {
            const string MessageTypes = "TheMessageTypes";
            const string Body = "TheMessageBody";

            var transportMessage = CreateTransportMessage(Body, "notused", null);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                MessageTypes,
                () => "notused");

            amqMessage.Text.Should().Be(Body);
            amqMessage.NMSType.Should().Be(MessageTypes);
        }

        [Test]
        public void Convert_AssignsConfiguredReplyToAddress()
        {
            var transportMessage = CreateTransportMessage("notused", "notused", null);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                "notused",
                () => "notused");

            amqMessage.NMSReplyTo.As<IQueue>().QueueName.Should().Be(ActiveMqReplyToQueue);
        }

        [Test]
        public void Convert_WhenIntentIsToRequest_ShouldAssignTheCorrelationIdWithAddressEncodedAsCorrelationId()
        {
            const string CorrelationId = "TheCorrelationId";
            const string ReplyToAddress = "TheReplyToAddress@Endpoint";
            const string ExpectedResult = "TheCorrelationId_BridgeAddressMarker_TheReplyToAddress@Endpoint";

            var transportMessage = CreateTransportMessage("notused", ReplyToAddress, CorrelationId);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                "notused",
                () => "notused");

            amqMessage.NMSCorrelationID.Should().Be(ExpectedResult);
        }

        [Test]
        public void Convert_WhenIntentIsToReply_ShouldAssignTheCorrelationIdWithoutAddressEncodedAsCorrelationId()
        {
            const string CorrelationId = "TheCorrelationId_BridgeAddressMarker_TheReplyToAddress@Endpoint";
            const string ReplyToAddress = "TheReplyToAddress@Endpoint";
            const string ExpectedResult = "TheCorrelationId";

            var transportMessage = CreateTransportMessage("notused", ReplyToAddress, CorrelationId);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                "notused",
                () => "notused");

            amqMessage.NMSCorrelationID.Should().Be(ExpectedResult);
        }

        [Test]
        public void Convert_WhenCorrelationIdHasNoEncodedAddress_ThenDefaultAddressIsAssigned()
        {
            const string CorrelationId = "TheCorrelationId";
            const string DefaultAddress = "TheDefaultAddress";

            var transportMessage = CreateTransportMessage("notused", "notused", CorrelationId);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                "notused",
                () => DefaultAddress);

            amqMessage.NMSDestination.IsQueue.Should().BeTrue();
            amqMessage.NMSDestination.As<IQueue>().QueueName.Should().Be(DefaultAddress);
        }

        [Test]
        public void Convert_WhenCorrelationIdHasEncodedAddress_ThenEncodedAddressIsAssigned()
        {
            const string CorrelationId = "TheCorrelationId_BridgeAddressMarker_TheEncodedAddress";
            const string DefaultAddress = "TheDefaultAddress";
            const string EncodedAddress = "TheEncodedAddress";

            var transportMessage = CreateTransportMessage("notused", "notused", CorrelationId);

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                "notused",
                () => DefaultAddress);

            amqMessage.NMSDestination.IsQueue.Should().BeTrue();
            amqMessage.NMSDestination.As<IQueue>().QueueName.Should().Be(EncodedAddress);
        }

        [Test]
        public void Convert_WhenMessageIntentIsPublish_ThenDestinationIsTopic()
        {
            const string CorrelationId = "TheCorrelationId";
            const string DefaultAddress = "TheDefaultAddress";
            const string MessageType = "theMessageType";

            var transportMessage = CreateTransportMessage("notused", "notused", CorrelationId);
            transportMessage.MessageIntent = MessageIntentEnum.Publish;

            var amqMessage = this.testee.CreateActiveMqTextMessageFromMsmqTransportMessage(
                transportMessage,
                MessageType,
                () => DefaultAddress);

            amqMessage.NMSDestination.IsTopic.Should().BeTrue();
            amqMessage.NMSDestination.As<ITopic>().TopicName.Should().Be("VirtualTopic." + MessageType);
        }
        
        private static TransportMessage CreateTransportMessage(string body, string replyToAddress, string correlationId)
        {
            return new TransportMessage
                                       {
                                           Body = Encoding.UTF8.GetBytes(body),
                                           ReplyToAddress = Address.Parse(replyToAddress),
                                           CorrelationId = correlationId
                                       };
        }
    }
}