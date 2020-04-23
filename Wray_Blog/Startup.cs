using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wray_Blog.Startup))]
namespace Wray_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
