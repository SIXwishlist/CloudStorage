using Microsoft.Owin;
using Owin;
using System.Diagnostics;

[assembly: OwinStartup(typeof(OneDriveFinal.Startup))]

namespace OneDriveFinal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            Debug.WriteLine("Print from Startup.cs");

            ConfigureAuth(app);
        }
    }
}
