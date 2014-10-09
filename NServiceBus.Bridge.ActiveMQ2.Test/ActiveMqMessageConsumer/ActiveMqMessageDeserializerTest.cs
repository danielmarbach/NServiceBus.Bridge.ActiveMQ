using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apache.NMS.ActiveMQ.Commands;
using FakeItEasy;
using FluentAssertions;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
using NServiceBus.Serialization;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test.ActiveMqMessageConsumer
{
    [TestFixture]
    public class ActiveMqMessageDeserializerTest
    {
        private IMessageSerializer nsbMessageSerializer;

        private ActiveMqMessageDeserializer testee;

        [SetUp]
        public void SetUp()
        {
            this.nsbMessageSerializer = A.Fake<IMessageSerializer>();
            this.testee = new ActiveMqMessageDeserializer(this.nsbMessageSerializer);
        }

        [Test]
        public void DeserializeMessage()
        {
            const string Content = "The Message expectedContent";
            var expectedResult = new object();
            var message = new ActiveMQTextMessage(Content);
            var type = typeof(ActiveMqMessageDeserializerTest);

            A.CallTo(() => this.nsbMessageSerializer.Deserialize(A.Dummy<Stream>(), A.Dummy<IList<Type>>()))
                .WhenArgumentsMatch(this.VerifyDeserializationArguments(Content, type))
                .Returns(new[] { expectedResult });

            var result = this.testee.DeserializeMessage(message, type);

            result.Should().Be(expectedResult);
        }

        private Func<ArgumentCollection, bool> VerifyDeserializationArguments(string expectedContent, Type type)
        {
            return args =>
                {
                    var stream = (MemoryStream)args[0];
                    using (var reader = new StreamReader(stream))
                    {
                        var streamContent = reader.ReadToEnd();
                        if (streamContent != expectedContent)
                        {
                            return false;
                        }
                    }

                    return type == ((IList<Type>)args[1]).Single();
                };
        }
    }
}