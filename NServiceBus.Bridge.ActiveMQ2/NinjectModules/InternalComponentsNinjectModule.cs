using Ninject.Modules;
using NServiceBus.Bridge.ActiveMQ2.ActiveMqMessageConsumer;
using NServiceBus.Bridge.ActiveMQ2.Msmq;

namespace NServiceBus.Bridge.ActiveMQ2.NinjectModules
{
    public class InternalComponentsNinjectModule : NinjectModule
    {
        public override void Load()
        {
            ////this.Kernel.Bind(x => 
            ////    x.FromThisAssembly().SelectAllClasses().WithoutAnyInterface()
            ////     .Excluding<MaxNumberOfParallelExecutionsGuard>()
            ////     .BindToSelf()
            ////     .ConfigureFor<MsmqToActiveMqMessageConverter>(c => c.WithConstructorArgument("replyToQueue", "ActiveMqBridge" /* Get from config */))
            ////     .ConfigureFor<ActiveMqListener>(
            ////        c => c.WithConstructorArgument("replyToQueue", "ActiveMqBridge")
            ////              .WithConstructorArgument("errorQueue", "ActiveMqBridge.Error")));
        }
    }
}