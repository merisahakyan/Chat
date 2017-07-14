using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Simple_Chat.Startup))]
namespace Simple_Chat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
           // GlobalHost.HubPipeline.AddModule(new RejoingGroupPipelineModule());
        }
    }
}
