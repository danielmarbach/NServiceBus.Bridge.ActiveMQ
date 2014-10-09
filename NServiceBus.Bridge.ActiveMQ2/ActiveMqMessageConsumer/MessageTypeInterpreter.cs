using System;
using System.Linq;

namespace NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer
{
    public class MessageTypeInterpreter
    {
        public virtual Type GetTypeFromNmsType(string nmsType)
        {
            if (string.IsNullOrEmpty(nmsType))
            {
                return null;
            }

            return Type.GetType(nmsType) ?? ScanAllAssembliesForType(nmsType);
        }

        private static Type ScanAllAssembliesForType(string nmsType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(nmsType))
                .FirstOrDefault(type => type != null);
        }
    }
}