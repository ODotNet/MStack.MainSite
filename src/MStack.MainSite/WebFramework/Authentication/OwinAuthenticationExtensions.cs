using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using MStack.MainSite.Controllers;
using MStack.MainSite.WebFramework.Authentication;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MStack.MainSite.WebFramework.Authentication
{
    public static class OwinAuthenticationExtensions
    {
        public static string CookieDomain = ConfigurationManager.AppSettings["CookieDomain"];
        public static string AuthCookieName = ConfigurationManager.AppSettings["AuthCookieName"];

        public static IAppBuilder UseMStackOwinAuthentication(this IAppBuilder app)
        {
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login"),
                //CookieName = AuthCookieName,
                CookieDomain = CookieDomain,
                CookieHttpOnly = false,
                CookieSecure = CookieSecureOption.Never,
                //Provider = new CookieAuthenticationProvider
                //{
                //    OnApplyRedirect = ctx =>
                //    {
                //        if (!IsAjaxRequest(ctx.Request))
                //        {
                //            ctx.Response.Redirect(ctx.RedirectUri);
                //        }
                //        else
                //        {
                //            ctx.Response.StatusCode = 401;
                //            ctx.Response.Headers.Add("loginUrl", new string[] { ctx.RedirectUri });
                //        }
                //    },
                //    OnValidateIdentity = SessionKeyStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                //    validateInterval: TimeSpan.FromSeconds(300),
                //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //},
            });
            return app;
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }



    public class ApplicationUser : User, IUser<Guid>
    {
        public ApplicationUser()
        {

        }

        public ApplicationUser(User user)
        {
            this.Id = user.Id;
            this.UserName = user.Email;
            this.DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName;
            this.Email = user.Email;
            this.PasswordHash = user.PasswordHash;
            this.SecurityStamp = user.SecurityStamp;
            this.EmailConfirmed = user.EmailConfirmed;
        }

        //public ApplicationUser(SessionObject sessionObject)
        //    : this(sessionObject.LogonUser)
        //{
        //    this.SessionKey = sessionObject.SessionKey;
        //}

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            //userIdentity.AddClaim(new Claim(AuthConstants.SessionKey, this.SessionKey));
            //userIdentity.AddClaim(new Claim(AuthConstants.LogonUser, JsonConvert.SerializeObject(this)));
            return userIdentity;
        }

        public List<UserClaim> Claims { get; set; }
    }

    public static class AuthConstants
    {
        public const string SessionKey = "SessionKey";
        public const string DisplayName = "DisplayName";
    }
}