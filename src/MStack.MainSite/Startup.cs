using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using MStack.MainSite.WebFramework.Authentication;
using Microsoft.AspNet.Identity;
using MStack.MainSite.Controllers;
using KatanaContrib.Security.Github;
using KatanaContrib.Security.LinkedIn;

[assembly: OwinStartup(typeof(MStack.MainSite.Startup))]

namespace MStack.MainSite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCors(CorsOptions.AllowAll);

            app.UseMStackOwinAuthentication();

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //app.UseQQConnectAuthentication(new Microsoft.Owin.Security.QQ.QQConnectAuthenticationOptions()
            //{
            //    AppId = "101294257",
            //    AppSecret = "10ef6f7c0f935d892c058f240474645d",
            //    ReturnEndpointPath = "/signin-qqconnect",
            //    Caption = "QQ登录"
            //    //SignInAsAuthenticationType = "QQ登录",
            //});
            app.UseQQConnectAuthentication(appId: "101294257", appSecret: "10ef6f7c0f935d892c058f240474645d");

            app.UseMicrosoftAccountAuthentication(new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationOptions
            {
                ClientId = "000000004018CC02",
                ClientSecret = "r-qtEQMR8-PIAWc9Zlfp2unOggVUiCuH",
                CallbackPath = new PathString("/signin-microsoft"),
                Caption = "Windows帐号登录",

            });

            app.UseGithubAuthentication(new GithubAuthenticationOptions()
            {
                ClientId = "1867a5f61a2201829162",
                ClientSecret = "95a1b9c9e10b8d614f3416f308d49d7466e0ec8e",
                CallbackPath = new PathString("/signin-github"),
                Caption = "GitHub"
            });

            app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions()
            {
                AppId = "759bsrm6x5i6er",
                AppSecret = "o9Ht8ktjw6tHhoXm",
                CallbackPath = new PathString("/signin-linkedin"),
                Caption = "LinkedIn(领英)",
            });
        }
    }
}
