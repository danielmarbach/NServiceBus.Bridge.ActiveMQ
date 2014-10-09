using System;

namespace NServiceBus.Bridge.ActiveMQ2
{
    public class CorrelationIdAndAddressTranscoder
    {
        public const string AddressMarker = "_BridgeAddressMarker_";

        public virtual string EncodeAddressToCorrelationId(string receivedCorrelationId, string correlationId, string replyToAddress)
        {
            if (HasBridgeMarker(receivedCorrelationId))
            {
                return correlationId;
            }

            return string.Concat(correlationId, AddressMarker, replyToAddress);
        }

        public virtual CorrelationIdAndAddress DecodeCorrelationId(string receivedCorrelationId)
        {
            if (HasBridgeMarker(receivedCorrelationId))
            {
                return ExtractCorrelationIdAndAddress(receivedCorrelationId);
            }

            return new CorrelationIdAndAddress(receivedCorrelationId, null);
        }

        public virtual CorrelationIdAndAddress TranscodeCorrelationIdAndAddress(string correlationId, string replyToAddress)
        {
            if (HasBridgeMarker(correlationId))
            {
                return ExtractCorrelationIdAndAddress(correlationId);
            }

            return new CorrelationIdAndAddress(string.Concat(correlationId, AddressMarker, replyToAddress), null);
        }

        private static bool HasBridgeMarker(string receivedCorrelationId)
        {
            return receivedCorrelationId.Contains(AddressMarker);
        }

        private static CorrelationIdAndAddress ExtractCorrelationIdAndAddress(string correlationId)
        {
            var decodedParts = correlationId.Split(new[] { AddressMarker }, StringSplitOptions.None);

            correlationId = decodedParts[0];
            var address = decodedParts[1];

            return new CorrelationIdAndAddress(correlationId, address);
        }
    }
}