using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.Unicast;

namespace NServiceBus.Bridge.ActiveMQ2
{
    public static class UnicastBusExtensions
    {
        private static readonly MethodInfo GetAddressForMessageTypeMethodInfo = typeof(UnicastBus).GetMethod(
            "GetAddressForMessageType",
            BindingFlags.Instance | BindingFlags.NonPublic);

        public static IList<Address> GetAddressForMessageType(this UnicastBus bus, Type type)
        {
            var result = GetAddressForMessageTypeMethodInfo.Invoke(bus, new object[] { type }) as List<Address>;
            return result;
        }
    }
}