using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Ninject.Modules;

namespace NServiceBus.Bridge.ActiveMQ2.NinjectModules
{
    public class ActiveMqConncetionFactoryNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IConnectionFactory>().ToMethod(
                ctx =>
                    {
                        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ActiveMq"];
                        return new ConnectionFactory(connectionString.ConnectionString)
                                   {
                                       AcknowledgementMode = AcknowledgementMode.ClientAcknowledge
                                   };
                    }).InSingletonScope();
        }
    }
}