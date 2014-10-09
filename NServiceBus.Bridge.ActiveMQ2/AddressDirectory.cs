using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NServiceBus.Unicast;

namespace NServiceBus.Bridge.ActiveMQ2
{
    public class AddressDirectory
    {
        private static readonly MethodInfo GetAddressForMessageTypeMethodInfo = typeof(UnicastBus).GetMethod(
            "GetAddressForMessageType",
            BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IBus bus;

        public AddressDirectory(IBus bus)
        {
            this.bus = bus;
        }

        public virtual Address GetAddress(Type messageType)
        {
            return (GetAddressForMessageTypeMethodInfo.Invoke(this.bus, new object[] { messageType }) as List<Address>).Single();
        }
    }
}