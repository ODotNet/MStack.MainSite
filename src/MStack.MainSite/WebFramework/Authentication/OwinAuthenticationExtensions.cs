using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
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
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login"),
                //CookieName = AuthCookieName,
                CookieDomain = CookieDomain,
                CookieHttpOnly = false,
                CookieSecure = CookieSecureOption.Never,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        else
                        {
                            ctx.Response.StatusCode = 401;
                            ctx.Response.Headers.Add("loginUrl", new string[] { ctx.RedirectUri });
                        }
                    },
                    OnValidateIdentity = SessionKeyStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    validateInterval: TimeSpan.FromSeconds(300),
                    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
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

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserRepository(context));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                //AllowOnlyAlphanumericUserNames = false,
                //RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            //manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is: {0}"
            //});
            //manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is: {0}"
            //});
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationUser : IUser
    {
        public ApplicationUser()
        {

        }

        //public ApplicationUser(User user)
        //{
        //    this.LogonUser = user;
        //    this.Id = user.Id.ToString();
        //    this.UserId = user.Id;
        //    this.UserName = user.Email;
        //    this.DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.Email : user.DisplayName;
        //}

        //public ApplicationUser(SessionObject sessionObject)
        //    : this(sessionObject.LogonUser)
        //{
        //    this.SessionKey = sessionObject.SessionKey;
        //}

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(AuthConstants.SessionKey, this.SessionKey));
            //userIdentity.AddClaim(new Claim(AuthConstants.LogonUser, JsonConvert.SerializeObject(this)));
            return userIdentity;
        }

        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string SessionKey { get; set; }

        public string Email { get; set; }
    }

    public static class AuthConstants
    {
        public const string SessionKey = "SessionKey";
        public const string DisplayName = "DisplayName";
    }

    public class SessionKeyStampValidator
    {
        //public static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Can be used as the ValidateIdentity method for a CookieAuthenticationProvider which will check a user's security stamp after validateInterval
        /// Rejects the identity if the stamp changes, and otherwise will call regenerateIdentity to sign in a new ClaimsIdentity
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="validateInterval"></param>
        /// <param name="regenerateIdentity"></param>
        /// <returns></returns>
        public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager, TUser>(TimeSpan validateInterval, Func<TManager, TUser, Task<ClaimsIdentity>> regenerateIdentity)
            where TManager : UserManager<TUser, string>
            where TUser : class, IUser<string>
        {
            return OnValidateIdentity<TManager, TUser, string>(validateInterval, regenerateIdentity, (id) => id.GetUserId());
        }

        /// <summary>
        /// Can be used as the ValidateIdentity method for a CookieAuthenticationProvider which will check a user's security stamp after validateInterval
        /// Rejects the identity if the stamp changes, and otherwise will call regenerateIdentity to sign in a new ClaimsIdentity
        /// </summary>
        /// <typeparam name="TManager"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="validateInterval"></param>
        /// <param name="regenerateIdentityCallback"></param>
        /// <param name="getUserIdCallback"></param>
        /// <returns></returns>
        public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager, TUser, TKey>(TimeSpan validateInterval, 
            Func<TManager, TUser, Task<ClaimsIdentity>> regenerateIdentityCallback, 
            Func<ClaimsIdentity, TKey> getUserIdCallback)
            where TManager : UserManager<TUser, TKey>
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return async (context) =>
            {
                DateTimeOffset currentUtc = DateTimeOffset.UtcNow;
                if (context.Options != null && context.Options.SystemClock != null)
                {
                    currentUtc = context.Options.SystemClock.UtcNow;
                }
                DateTimeOffset? issuedUtc = context.Properties.IssuedUtc;

                // Only validate if enough time has elapsed
                bool validate = (issuedUtc == null);
                if (issuedUtc != null)
                {
                    TimeSpan timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                    validate = timeElapsed > validateInterval;
                }

                var claims = context.Identity.Claims.ToDictionary(x => x.Type, x => x.Value);

                if (validate || !claims.ContainsKey(AuthConstants.SessionKey) || claims[AuthConstants.SessionKey] == null)
                {
                    bool reject = true;
                    var oldSessionKey = claims[AuthConstants.SessionKey];
                    if (!string.IsNullOrWhiteSpace(oldSessionKey))
                    {
                        //var response = AccountApiClient.SessionValidate(new SessionQuery() { SessionKey = oldSessionKey });
                        //if (response.Success)
                        //{
                        //    reject = false;
                        //    TUser user = new ApplicationUser(response.ResponseResult) as TUser;

                        //    // Regenerate fresh claims if possible and resign in
                        //    if (regenerateIdentityCallback != null)
                        //    {
                        //        var manager = context.OwinContext.GetUserManager<TManager>();
                        //        ClaimsIdentity identity = await regenerateIdentityCallback.Invoke(manager, user);
                        //        if (identity != null)
                        //        {
                        //            context.OwinContext.Authentication.SignIn(identity);
                        //        }
                        //    }
                        //}
                        //if (reject)
                        //{
                        //    PermissionService.Instance.RemovePermissionCache(oldSessionKey);//移除User Permission Cache
                        //}
                        ////var sysUser = new RemoteUserService().GetUser(claims.ContainsKey(AuthConstants.UserId) ? claims[AuthConstants.UserId] : string.Empty);

                        //if (sysUser != null)
                        //{
                        //    reject = false;
                        //    context.Request.Set<string>(AuthConstants.UserId, sysUser.SysNO.ToString());
                        //    //userIdentity.AddClaim(new Claim(AuthConstants.SessionKey, this.SessionKey));
                        //    //userIdentity.AddClaim(new Claim(AuthConstants.DisplayName, this.DisplayName));
                        //    //userIdentity.AddClaim(new Claim(AuthConstants.Logonuser, JsonConvert.SerializeObject(this.User)));

                        //}
                    }

                    if (reject)
                    {
                        context.RejectIdentity();
                        context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
                    }
                }
            };
        }
    }
}