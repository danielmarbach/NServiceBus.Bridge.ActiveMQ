using System;
using System.IO;
using System.Linq;
using System.Text;
using Apache.NMS;
using NServiceBus.Serialization;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class ActiveMqMessageDeserializer
    {
        private readonly IMessageSerializer messageSerializer;

        public ActiveMqMessageDeserializer(IMessageSerializer messageSerializer)
        {
            this.messageSerializer = messageSerializer;
        }

        public virtual object DeserializeMessage(ITextMessage amqMessage, Type type)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(amqMessage.Text)))
            {
                return this.messageSerializer.Deserialize(stream, new[] { type }).Single();
            }
        }
    }
}