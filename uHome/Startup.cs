using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(uHome.Startup))]
namespace uHome
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
