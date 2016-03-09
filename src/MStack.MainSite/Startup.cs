using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using MStack.MainSite.WebFramework.Authentication;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(MStack.MainSite.Startup))]

namespace MStack.MainSite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCors(CorsOptions.AllowAll);
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseMStackOwinAuthentication();

            app.UseQQConnectAuthentication(appId: "101294257", appSecret: "10ef6f7c0f935d892c058f240474645d");
        }
    }
}
