namespace NServiceBus.Bridge.ActiveMQ2
{
    public class CorrelationIdAndAddress
    {
        public CorrelationIdAndAddress(string correlationId, string address)
        {
            this.CorrelationId = correlationId;
            this.Address = address;
        }

        public string Address { get; private set; }

        public string CorrelationId { get; private set; }
    }
}