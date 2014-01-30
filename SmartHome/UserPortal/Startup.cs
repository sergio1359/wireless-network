using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserPortal.Startup))]
namespace UserPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
