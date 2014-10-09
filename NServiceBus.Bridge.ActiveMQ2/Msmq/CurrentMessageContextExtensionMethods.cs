using System.Reflection;
using NServiceBus.Unicast;

namespace NServiceBus.Bridge.ActiveMQ2.Msmq
{
    public static class CurrentMessageContextExtensionMethods
    {
        private static readonly FieldInfo TransportMessageFieldInfo = typeof(MessageContext).GetField(
            "transportMessage",
            BindingFlags.Instance | BindingFlags.NonPublic);
        
        public static TransportMessage GetTransportMessage(this IMessageContext messageContext)
        {
            return TransportMessageFieldInfo.GetValue(messageContext) as TransportMessage;
        }
    }
}