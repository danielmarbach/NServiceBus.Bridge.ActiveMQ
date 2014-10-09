//using System.Collections.Generic;
//using FakeItEasy;
//using FluentAssertions;
//using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
//using NUnit.Framework;

//namespace NServiceBus.Bridge.ActiveMQ2.Test.ActiveMqMessageConsumer
//{
//    [TestFixture]
//    public class MessageSenderTest
//    {
//        private IBus bus;

//        private MessageSender testee;

//        private AddressDirectory addressDictionary;

//        private Dictionary<string, string> headers;

//        [SetUp]
//        public void SetUp()
//        {
//            this.bus = A.Fake<IBus>();
//            this.addressDictionary = A.Fake<AddressDirectory>();

//            ExtensionMethods.SetHeaderAction = this.StoreHeader;

//            this.headers = new Dictionary<string, string>();

//            this.testee = new MessageSender(this.bus, this.addressDictionary);
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsEvent_ShouldPublishMessage()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(EventMessage);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            A.CallTo(() => this.bus.Publish(message)).MustHaveHappened();
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsEvent_ShouldSetCorrelationIdAndId()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(EventMessage);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            this.headers.Should()
//                .Contain(new KeyValuePair<string, string>(Headers.MessageId, CorrelationId))
//                .And.Contain(new KeyValuePair<string, string>(Headers.CorrelationId, CorrelationId))
//                .And.HaveCount(2);
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsMessage_ShouldSend()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(Message);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            A.CallTo(() => this.addressDictionary.GetAddress(type)).Returns(new Address("AnyQueue", "AnyMachine"));

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            A.CallTo(() => this.bus.Send(A<Address>._, message)).MustHaveHappened();
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsMessage_ShouldSetCorrelationIdOnly()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(Message);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            this.headers.Should()
//                .Contain(new KeyValuePair<string, string>(Headers.CorrelationId, CorrelationId))
//                .And.HaveCount(1);
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsCommand_ShouldSend()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(Command);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            A.CallTo(() => this.addressDictionary.GetAddress(type)).Returns(new Address("AnyQueue", "AnyMachine"));

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            A.CallTo(() => this.bus.Send(A<Address>._, message)).MustHaveHappened();
//        }

//        [Test]
//        public void SendMessage_WhenMessageIsCommand_ShouldSetCorrelationIdAndId()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(Command);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            this.headers.Should()
//                .Contain(new KeyValuePair<string, string>(Headers.MessageId, CorrelationId))
//                .And.Contain(new KeyValuePair<string, string>(Headers.CorrelationId, CorrelationId))
//                .And.HaveCount(2);
//        }

//        [Test]
//        public void SendMessage_WhenSendWithNoAddressDefined_SendsMessageToDefaultAddress()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            var message = new object();
//            var type = typeof(Message);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, null);
//            var address = new Address("DefaultQueue", "Machine");

//            A.CallTo(() => this.addressDictionary.GetAddress(type)).Returns(address);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            A.CallTo(() => this.bus.Send(address, message)).MustHaveHappened();
//        }

//        [Test]
//        public void SendMessage_WhenSendWithAddressDefined_SendsMessageThisAddress()
//        {
//            const string CorrelationId = "TheCorrelationId";
//            const string ReplyToAddress = "TheAddress";
//            var message = new object();
//            var type = typeof(Command);
//            var correlationIdAndAddress = new CorrelationIdAndAddress(CorrelationId, ReplyToAddress);

//            this.testee.SendMessage(message, correlationIdAndAddress, type);

//            A.CallTo(() => this.bus.Send(Address.Parse(ReplyToAddress), message)).MustHaveHappened();
//        }

//        private void StoreHeader(object message, string key, string value)
//        {
//            this.headers.Add(key, value);
//        }

//        [Event]
//        private class EventMessage
//        {
//        }

//        [Message]
//        private class Message
//        {
//        }

//        [Command]
//        private class Command
//        {
//        }
//    }
//}