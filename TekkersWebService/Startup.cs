using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TekkersWebService.Startup))]

namespace TekkersWebService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}