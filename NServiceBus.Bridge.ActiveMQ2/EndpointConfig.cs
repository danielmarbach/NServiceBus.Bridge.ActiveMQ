using System;
using System.Reflection;
using Ninject;

namespace NServiceBus.Bridge.ActiveMQ2
{
    public sealed class EndpointConfig : IConfigureThisEndpoint, IWantCustomLogging, AsA_Publisher, IWantCustomInitialization
    {
        private StandardKernel kernel;

        public void Init()
        {
            ////this.kernel = new StandardKernel();
            ////this.kernel.Load(Assembly.GetExecutingAssembly());

            ////SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);

            ////Configure.Serialization.Xml(s => s.Namespace("http://www.yournamespace.com"))
            ////         .DontWrapSingleMessages();

            ////Configure.With()
            ////         .DefineEndpointName("bridge")
            ////         .NinjectBuilder(this.kernel)
            ////         .Log4Net()
            ////         .DefiningMessagesAs(Conventions.Messages)
            ////         .DefiningEventsAs(Conventions.Events)
            ////         .DefiningCommandsAs(Conventions.Commands);
        }
    }
}
