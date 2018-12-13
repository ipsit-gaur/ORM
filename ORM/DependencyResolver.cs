using Autofac;
using ORM.Data;

namespace ORM
{
    internal class DependencyResolver
    {
        internal static IContainer Container { get; private set; }

        internal void Register()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(x => new SQLManager("ConnectionString")).As<IDataSourceManager>().SingleInstance();
            Container = containerBuilder.Build();
            Container.BeginLifetimeScope();
        }
    }
}
