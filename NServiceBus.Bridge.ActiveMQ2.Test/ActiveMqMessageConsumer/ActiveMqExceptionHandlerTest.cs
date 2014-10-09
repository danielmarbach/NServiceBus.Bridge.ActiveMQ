using System;
using System.Globalization;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using FakeItEasy;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test.ActiveMqMessageConsumer
{
    [TestFixture]
    public class ActiveMqExceptionHandlerTest
    {
        private IMessageProducer errorMessageProducer;

        private ActiveMqExceptionHandler testee;

        private ActiveMqMessageHandler messageHandler;

        [SetUp]
        public void SetUp()
        {
            this.errorMessageProducer = A.Fake<IMessageProducer>();
            this.messageHandler = A.Fake<ActiveMqMessageHandler>();

            this.testee = new ActiveMqExceptionHandler(this.messageHandler);
        }

        [Test]
        public void ProcessMessage_WhenMessageIsNotATextMessage_ThenErrorMessageIsSent()
        {
            const string Queue = "SomeQueue";
            var amqMessage = new ActiveMQBytesMessage
                                 {
                                     NMSTimestamp = DateTime.Now,
                                     Destination = new ActiveMQQueue(Queue)
                                 };

            this.testee.ProcessMessage(amqMessage, this.errorMessageProducer);

            this.ErrorMessageShouldBeSent(
                amqMessage, 
                exceptionMessage => exceptionMessage.StartsWith("System.InvalidCastException"), 
                "queue://" + Queue);
        }

        [Test]
        public void ProcessMessage_WhenExceptionOccurs_ThenErrorMessageIsSent()
        {
            const string Queue = "SomeQueue";
            const string Message = "TheExceptionMessage";
            var exception = new Exception(Message);

            var amqMessage = new ActiveMQTextMessage
            {
                NMSTimestamp = DateTime.Now,
                Destination = new ActiveMQQueue(Queue)
            };

            A.CallTo(() => this.messageHandler.ProcessMessage(amqMessage)).Throws(exception);

            this.testee.ProcessMessage(amqMessage, this.errorMessageProducer);

            this.ErrorMessageShouldBeSent(
                amqMessage,
                exceptionMessage => exceptionMessage.StartsWith("System.Exception: TheExceptionMessage"),
                "queue://" + Queue);
        }

        [Test]
        public void ProcessMessage_WhenNoExceptionOccurs_ThenNoErrorMessageIsSent()
        {
            const string Queue = "SomeQueue";

            var amqMessage = new ActiveMQTextMessage
            {
                NMSTimestamp = DateTime.Now,
                Destination = new ActiveMQQueue(Queue)
            };

            this.testee.ProcessMessage(amqMessage, this.errorMessageProducer);

            A.CallTo(() => this.messageHandler.ProcessMessage(amqMessage)).MustHaveHappened();
            A.CallTo(() => this.errorMessageProducer.Send(A.Dummy<Apache.NMS.IMessage>())).MustNotHaveHappened();
        }
        
        private void ErrorMessageShouldBeSent(Apache.NMS.IMessage message, Func<string, bool> errorMessageVerification, string expectedDestination)
        {
            A.CallTo(() => this.errorMessageProducer.Send(A.Dummy<Apache.NMS.IMessage>()))
                .WhenArgumentsMatch(
                    args =>
                        {
                            var sentMessage = (Apache.NMS.IMessage)args[0];
                            if (!sentMessage.Properties.Contains("Bridge.Error.Message")
                                || !sentMessage.Properties.Contains("Bridge.Error.OriginalDestination")
                                || !sentMessage.Properties.Contains("Bridge.Error.OriginalTimestamp")
                                || !sentMessage.Properties.Contains("Bridge.Error.OriginalMessageId"))
                            {
                                return false;
                            }

                            return errorMessageVerification((string)sentMessage.Properties["Bridge.Error.Message"]) &&
                                   sentMessage.Properties["Bridge.Error.OriginalDestination"].Equals(expectedDestination) &&
                                   sentMessage.Properties["Bridge.Error.OriginalTimestamp"].Equals(message.NMSTimestamp.ToString(CultureInfo.InvariantCulture)) &&
                                   sentMessage.Properties["Bridge.Error.OriginalMessageId"].Equals(message.NMSMessageId);
                        })
                .MustHaveHappened();
        }
    }
}