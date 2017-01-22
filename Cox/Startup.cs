using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cox.Startup))]
namespace Cox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
