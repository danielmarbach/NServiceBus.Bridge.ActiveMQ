using FluentAssertions;
using NUnit.Framework;

namespace NServiceBus.Bridge.ActiveMQ2.Test
{
    [TestFixture]
    public class CorrelationIdAndAddressEncoderTest
    {
        private CorrelationIdAndAddressTranscoder testee;

        [SetUp]
        public void SetUp()
        {
            this.testee = new CorrelationIdAndAddressTranscoder();
        }

        [Test]
        public void ReceivedCorrelationWithoutBridgeMarker_ShouldReturnCorrelationIdWithMarkerAndQueue()
        {
            var encodedCorrelationId = this.testee.EncodeAddressToCorrelationId("SomeCorrelationId", "SomeCorrelationId", "SomeQueue");

            encodedCorrelationId.Should().Be("SomeCorrelationId_BridgeAddressMarker_SomeQueue");
        }

        [Test]
        public void ReceivedCorrelationIdWithMarker_ShouldReturnCorrelationIdWithoutMarker()
        {
            var encodedCorrelationId = this.testee.EncodeAddressToCorrelationId("SomeCorrelationId_BridgeAddressMarker_SomeQueue", "SomeCorrelationId", "SomeQueue");

            encodedCorrelationId.Should().Be("SomeCorrelationId");
        }

        [Test]
        public void WhenNoAddressIsEncodedInCorrelationIdItIsNull()
        {
            const string CorrelationId = "SomeCorrelationId";

            var decodedCorrelationId = this.testee.DecodeCorrelationId(CorrelationId);

            decodedCorrelationId.CorrelationId.Should().Be(CorrelationId);
            decodedCorrelationId.Address.Should().BeNull();
        }

        [Test]
        public void WhenAddressIsEncodedInCorrelationId_ShouldDecodeAddress()
        {
            const string CorrelationId = "SomeCorrelationId_BridgeAddressMarker_SomeQueue";
            const string ExpectedCorrelationId = "SomeCorrelationId";
            const string ExpectedAddress = "SomeQueue";

            var decodedCorrelationId = this.testee.DecodeCorrelationId(CorrelationId);

            decodedCorrelationId.CorrelationId.Should().Be(ExpectedCorrelationId);
            decodedCorrelationId.Address.Should().Be(ExpectedAddress);
        }    
        
        [Test]
        public void WhenBridgeMarkerExistisWithingCorrelationIdWithEmptyAddress_ShouldExtractEmptyAddress()
        {
            const string CorrelationId = "SomeCorrelationId_BridgeAddressMarker_";
            const string ExpectedCorrelationId = "SomeCorrelationId";

            var decodedCorrelationId = this.testee.DecodeCorrelationId(CorrelationId);

            decodedCorrelationId.CorrelationId.Should().Be(ExpectedCorrelationId);
            decodedCorrelationId.Address.Should().BeEmpty();
        }
    }
}