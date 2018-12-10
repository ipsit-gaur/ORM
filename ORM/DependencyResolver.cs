using Autofac;

namespace ORM
{
    internal class DependencyResolver
    {
        internal static IContainer Container { get; private set; }

        internal void Register()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<SQLManager>().As<IDataSourceManager>().SingleInstance();
            Container = containerBuilder.Build();
            Container.BeginLifetimeScope();
        }
    }
}
