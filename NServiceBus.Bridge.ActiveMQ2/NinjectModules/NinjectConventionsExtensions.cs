using System.Linq;
using Ninject.Extensions.Conventions.Syntax;

namespace NServiceBus.Bridge.ActiveMQ2.NinjectModules
{
    public static class NinjectConventionsExtensions
    {
        public static IJoinExcludeIncludeBindSyntax WithoutAnyInterface(this IWhereSyntax syntax)
        {
            return syntax.Where(t => !t.GetInterfaces().Any());
        }
    }
}