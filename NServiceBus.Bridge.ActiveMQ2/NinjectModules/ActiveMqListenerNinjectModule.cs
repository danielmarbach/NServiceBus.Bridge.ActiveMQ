using Ninject.Modules;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;

namespace NServiceBus.Bridge.ActiveMQ2.NinjectModules
{
    public class ActiveMqListenerNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IWantToRunWhenBusStartsAndStops, ActiveMqListenerTaskScheduler>()
                .To<ActiveMqListenerTaskScheduler>()
                .WithConstructorArgument("pollingInterval", 1000 /* Get from config */);
            
            this.Bind<MaxNumberOfParallelExecutionsGuard>().ToSelf().WhenInjectedInto<ActiveMqListenerTaskScheduler>()
                .WithConstructorArgument("maxNumberOfRunningActions", 4 /* Get from config */);
        }        
    }
}