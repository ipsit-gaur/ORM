using Autofac;
using ORM.Data;
using ORM.Logger;
using ORM.SQLServer;

namespace ORM
{
    internal class DependencyResolver
    {
        internal static IContainer Container { get; private set; }

        internal void Register()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(x => new SQLServerManager("ConnectionString")).As<IDataSourceManager>().SingleInstance();
            containerBuilder.RegisterType<SQLServerQueryBuilder>().As<IQueryBuilder>();
            containerBuilder.RegisterType<LogManager>().As<LogManager>();
            Container = containerBuilder.Build();
            Container.BeginLifetimeScope();
        }
    }
}
