using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ORM.Reports.Startup))]
namespace ORM.Reports
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
