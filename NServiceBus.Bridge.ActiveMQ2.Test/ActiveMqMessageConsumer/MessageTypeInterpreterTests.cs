using FluentAssertions;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test.ActiveMqMessageConsumer
{
    [TestFixture]
    public class MessageTypeInterpreterTests
    {
        private MessageTypeInterpreter testee;

        [SetUp]
        public void SetUp()
        {
            this.testee = new MessageTypeInterpreter();
        }        

        [Test]
        public void GetType_IfInputNull_ReturnsNull()
        {
            var result = this.testee.GetTypeFromNmsType(null);

            result.Should().BeNull();
        }

        [Test]
        public void GetType_IfEmptyString_ReturnsNull()
        {
            var result = this.testee.GetTypeFromNmsType(string.Empty);

            result.Should().BeNull();
        }

        [Test]
        public void GetType_IfAssemblyQualifiedName_ReturnsType()
        {
            var name = this.GetType().AssemblyQualifiedName;

            var result = this.testee.GetTypeFromNmsType(name);

            result.Should().Be(this.GetType());
        }

        [Test]
        public void GetType_IfFullName_ReturnsType()
        {
            var name = this.GetType().FullName;

            var result = this.testee.GetTypeFromNmsType(name);

            result.Should().Be(this.GetType());
        }

        [Test]
        public void GetType_IfUninterpretableType_ReturnsNull()
        {
            const string Name = "SomeNonsenseType";

            var result = this.testee.GetTypeFromNmsType(Name);

            result.Should().BeNull();
        }
    }
}