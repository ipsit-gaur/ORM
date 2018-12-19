using Autofac;
using ORM.Data;
using ORM.SQL;

namespace ORM
{
    internal class DependencyResolver
    {
        internal static IContainer Container { get; private set; }

        internal void Register()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(x => new SQLManager("ConnectionString")).As<IDataSourceManager>().SingleInstance();
            containerBuilder.RegisterType<SQLQueryBuilder>().As<IQueryBuilder>();
            Container = containerBuilder.Build();
            Container.BeginLifetimeScope();
        }
    }
}
