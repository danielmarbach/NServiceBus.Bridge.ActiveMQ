using System.Linq;
using System.Reflection;

namespace NServiceBus.Bridge.ActiveMQ2.Msmq
{
    public class MsmqEventRegistration : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            RegisterXEvents(this.Bus);
            RegisterYEvents(this.Bus);
            RegisterZEvents(this.Bus);
        }

        public void Stop()
        {
        }

        private static void RegisterXEvents(IBus bus)
        {
            // SubscribeAllEventsFromAssembly(bus, typeof(TypeFromXContractAssembly).Assembly);
        }

        private static void RegisterYEvents(IBus bus)
        {
            // SubscribeAllEventsFromAssembly(bus, typeof(TypeFromYContractAssembly).Assembly);
        }

        private static void RegisterZEvents(IBus bus)
        {
            // SubscribeAllEventsFromAssembly(bus, typeof(TypeFromZContractAssembly).Assembly);
        }

        private static void SubscribeAllEventsFromAssembly(IBus bus, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => true /* Use here your conventions to detect events */))
            {
                bus.Subscribe(type);
            }
        }
    }
}