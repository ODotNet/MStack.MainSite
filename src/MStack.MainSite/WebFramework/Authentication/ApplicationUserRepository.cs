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
        public ISession Session
        {
            get
            {
                if (_session == null)
                    _session = NHSessionFactory.OpenSession();
                return _session;
            }
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TUser user)
        {
            user.DisplayName = user.Email;

            var userEntity = new User()
            {
                Email = user.Email,
                DisplayName = user.Email,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                UserName = user.Email,
                Id = user.Id
            };

            Session.Save(userEntity);
            Session.Flush();
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
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

        //private TUser ToAplicationUser(User user)
        //{
        //    return new TUser() { UserId = user.Id, Id = user.Id, DisplayName = user.DisplayName, Email = user.Email, PasswordHash = user.PasswordHash };
        //}

        //private TUser ToUserEntity(User user)
        //{
        //    return new TUser() { UserId = user.Id, Id = user.Id, DisplayName = user.DisplayName, Email = user.Email, PasswordHash = user.PasswordHash };
        //}

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
            return Task.FromResult(true);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
