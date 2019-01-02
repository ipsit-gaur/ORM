using Autofac;
using ORM.Configuration;
using ORM.Data;
using ORM.Logger;
using ORM.SQLServer;
using System.Configuration;

namespace ORM
{
    internal class DependencyResolver
    {
        internal static IContainer Container { get; private set; }

        internal void Register()
        {
            var containerBuilder = new ContainerBuilder();
            ORMConfiguration section = (ORMConfiguration)ConfigurationManager.GetSection("orm");
            containerBuilder.Register(x => new SQLServerManager(section)).As<IDataSourceManager>().SingleInstance();
            containerBuilder.RegisterType<SQLServerQueryBuilder>().As<IQueryBuilder>();
            containerBuilder.RegisterType<LogManager>().As<LogManager>();
            Container = containerBuilder.Build();
            Container.BeginLifetimeScope();
        }
    }
}
