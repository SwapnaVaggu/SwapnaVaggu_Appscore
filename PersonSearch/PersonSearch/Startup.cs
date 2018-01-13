using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PersonSearch.Startup))]
namespace PersonSearch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
