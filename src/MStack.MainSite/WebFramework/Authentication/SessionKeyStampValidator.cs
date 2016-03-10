using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MStack.MainSite.WebFramework.Authentication
{
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