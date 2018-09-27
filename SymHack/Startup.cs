using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SymHack.Startup))]
namespace SymHack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
