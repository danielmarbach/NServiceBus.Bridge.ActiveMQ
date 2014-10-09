using Apache.NMS.ActiveMQ.Commands;
using FakeItEasy;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test.ActiveMqMessageConsumer
{
    [TestFixture]
    public class ActiveMqMessageHandlerTest
    {
        private ActiveMqMessageHandler testee;

        private ActiveMqMessageDeserializer messageDeserializer;

        private MessageTypeInterpreter messageTypeInterpreter;

        private MessageSender messageSender;

        private CorrelationIdAndAddressTranscoder correlationIdAndAddressTranscoder;

        [SetUp]
        public void SetUp()
        {
            this.messageDeserializer = A.Fake<ActiveMqMessageDeserializer>();
            this.messageTypeInterpreter = A.Fake<MessageTypeInterpreter>();
            this.correlationIdAndAddressTranscoder = A.Fake<CorrelationIdAndAddressTranscoder>();
            this.messageSender = A.Fake<MessageSender>();

            this.testee = new ActiveMqMessageHandler(
                this.correlationIdAndAddressTranscoder,
                this.messageDeserializer,
                this.messageSender,
                this.messageTypeInterpreter);
        }

        [Test]
        public void ProcessMessage_OrchistratesParticipatingComponentsCorrectly()
        {
            const string NmsType = "TheNmsType";
            const string CorrelationId = "TheCorrelationId";
            const string ReplyToQueue = "TheReplyToQueue";

            var type = typeof(object);
            var deserializedMessage = new object();
            var correlationIdAndAddress = new CorrelationIdAndAddress(null, null);
            var amqMessage = new ActiveMQTextMessage
            {
                NMSType = NmsType,
                CorrelationId = CorrelationId,
                NMSReplyTo = new ActiveMQQueue(ReplyToQueue),
                NMSDestination = new ActiveMQQueue("Doesn't matter"),
            };

            A.CallTo(() => this.messageTypeInterpreter.GetTypeFromNmsType(NmsType)).Returns(type);
            A.CallTo(() => this.messageDeserializer.DeserializeMessage(amqMessage, type)).Returns(deserializedMessage);
            A.CallTo(() => this.correlationIdAndAddressTranscoder.TranscodeCorrelationIdAndAddress(CorrelationId, ReplyToQueue))
                .Returns(correlationIdAndAddress);

            this.testee.ProcessMessage(amqMessage);

            A.CallTo(() => this.messageSender.SendMessage(deserializedMessage, correlationIdAndAddress, type))
                .MustHaveHappened();
        }
    }
}