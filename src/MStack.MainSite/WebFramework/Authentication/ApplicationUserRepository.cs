using Microsoft.AspNet.Identity;
using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Linq;
using NHibernate;
using System.Security.Claims;

namespace MStack.MainSite.WebFramework.Authentication
{
    public class MyUserStore<TUser> : IUserLoginStore<TUser, Guid>, IUserClaimStore<TUser, Guid>, IUserRoleStore<TUser, Guid>, IUserPasswordStore<TUser, Guid>, IUserLockoutStore<TUser, Guid>,
        IUserSecurityStampStore<TUser, Guid>, IUserStore<TUser, Guid>, IUserEmailStore<TUser, Guid>, IUserPhoneNumberStore<TUser, Guid>, IUserTwoFactorStore<TUser, Guid>, IDisposable
        where TUser : ApplicationUser, new()
    {
        //public MyUserStore(ISession session)
        //{
        //    this.Session = session;
        //}

        private ISession _session = null;
        private ITransaction _trans = null;
        public ISession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = NHSessionFactory.OpenSession();
                    _trans = _session.BeginTransaction();
                }
                return _session;
            }
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            user.Claims.Add(new UserClaim()
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return Task.FromResult<int>(0);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            Session.SaveOrUpdate(new UserLogin() { UserId = user.Id, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });
            return Task.FromResult<int>(0);
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TUser user)
        {
            user.DisplayName = user.Email;

            var userEntity = GetUserFromTUser(user);

            Session.Save(userEntity);
            Session.Flush();
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TUser user)
        {
            var userEntity = Session.Get<User>(user.Id);
            if (userEntity != null)
                Session.Delete(userEntity);
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            if (_trans != null && !_trans.WasCommitted)
            {
                _trans.Commit();
            }
            if (Session != null)
            {
                Session.Flush();
                if (Session.IsConnected)
                {
                    Session.Close();
                }

                Session.Dispose();
            }
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin = Session.QueryOver<UserLogin>().Where(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey).SingleOrDefault();
            if (userLogin == null)
                //throw new SystemException("第三方账户信息不存在");
                return Task.FromResult<TUser>(null);

            var user = Session.Get<User>(userLogin.UserId);

            return Task.FromResult(new ApplicationUser(user) as TUser);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            var user = Session.Query<User>().SingleOrDefault(x => x.Email == email);
            return Task.FromResult(new ApplicationUser(user) as TUser);
        }

        public Task<TUser> FindByIdAsync(Guid userId)
        {
            var userEntity = Session.Get<User>(userId);
            //var appUser = ToAplicationUser(userEntity);
            return Task.FromResult(new ApplicationUser(userEntity) as TUser);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var userEntity = Session.Query<User>().SingleOrDefault(x => x.Email == userName);

            if (userEntity == null)
                return Task.FromResult<TUser>(null);

            //var appUser = ToAplicationUser(userEntity);
            return Task.FromResult(new ApplicationUser(userEntity) as TUser);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            //throw new NotImplementedException();
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<UserLoginInfo> result = new List<UserLoginInfo>();
            foreach (UserLogin identityUserLogin in Session.Query<UserLogin>().Where(x => x.UserId == user.Id).ToList())
            {
                result.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey));
            }

            return Task.FromResult<IList<UserLoginInfo>>(result);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.LoginFailTimes + 1);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var info = Session.Query<UserLogin>().SingleOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            if (info != null)
            {
                Session.Delete(info);
            }

            return Task.FromResult<int>(0);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TUser user)
        {
            if ((object)user == null)
            {
                throw new ArgumentNullException("user");
            }
            var userEntity = Session.Get<User>(user.Id);
            GetUserFromTUser(user, userEntity);
            Session.Update(userEntity);
            //this.Context.Update(user);
            //Context.Flush();

            return Task.FromResult(0);
        }

        private User GetUserFromTUser(TUser user, User userEntity = null)
        {
            if (userEntity == null)
                userEntity = new User();

            userEntity.Id = user.Id;
            userEntity.Email = user.Email;
            userEntity.DisplayName = user.Email;
            userEntity.PasswordHash = user.PasswordHash;
            userEntity.SecurityStamp = user.SecurityStamp;
            userEntity.UserName = user.Email;
            userEntity.EmailConfirmed = user.EmailConfirmed;

            return userEntity;
        }
    }
}
