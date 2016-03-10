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
    public class UserStore : MStackRepository<Guid>//, IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IQueryableUserStore<TUser>, IUserStore<TUser>, IUserLockoutStore<TUser, string>, IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>, IUserTwoFactorStore<TUser, string>, IDisposable
    {
        public UserStore(ISession session) : base(session)
        {

        }

        #region IUserStore
        public Task CreateAsync(ApplicationUser user)
        {
            var userEntity = new User()
            {
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                DisplayName = user.DisplayName
            };
            Insert(userEntity);
            Session.Flush();

            user.Id = userEntity.Id.ToString();
            user.DisplayName = userEntity.DisplayName;
            return Task.Run(() => { });
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            var userEntity = Get<User>(user.UserId);
            return Task.Run(() => { Delete<User>(userEntity); });
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var userEntity = Get<User>(Guid.Parse(userId));
            return Task.Run<ApplicationUser>(() =>
            {
                return new ApplicationUser()
                {
                    Id = userId,
                    UserId = userEntity.Id,
                    Email = userEntity.Email,
                    DisplayName = userEntity.DisplayName
                };
            });
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var userEntity = Session.Query<User>().SingleOrDefault(x => x.Email == userName);

            return Task.FromResult<ApplicationUser>(userEntity == null ? null : new ApplicationUser()
            {
                Id = userEntity.Id.ToString(),
                UserId = userEntity.Id,
                Email = userEntity.Email,
                DisplayName = userEntity.DisplayName,
                UserName = userEntity.Email
            });
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Session.Flush();
            Session.Close();
            Session.Dispose();
            //throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            //var userEntity = new User() { Email = user.Email, PasswordHash = passwordHash };
            //Insert(userEntity);
            //return Task.Run(() => { user.PasswordHash = passwordHash; });
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.Run<string>(() => { return user.PasswordHash; });
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.Run<bool>(() => false);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MyUserStore<TUser> : IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>, IUserPasswordStore<TUser>, IUserLockoutStore<TUser, string>,
        IUserSecurityStampStore<TUser>, IUserStore<TUser>, IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>, IUserTwoFactorStore<TUser, string>, IDisposable
        where TUser : ApplicationUser, new()
    {
        public MyUserStore(ISession session)
        {
            this.Session = session;
        }

        public ISession Session { get; private set; }

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
            var userEntity = new User()
            {
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                DisplayName = user.DisplayName
            };
            Session.Save(userEntity);
            Session.Flush();

            user.Id = userEntity.Id.ToString();
            user.DisplayName = userEntity.DisplayName;
            return Task.Run(() => { });
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

        public Task<TUser> FindByIdAsync(string userId)
        {
            var userEntity = Session.Get<User>(Guid.Parse(userId));
            var appUser = new TUser();

            appUser.Id = userId;
            appUser.UserId = userEntity.Id;
            appUser.Email = userEntity.Email;
            appUser.DisplayName = userEntity.DisplayName;
            return Task.FromResult(appUser);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var userEntity = Session.Query<User>().SingleOrDefault(x => x.Email == userName);

            if (userEntity == null)
                return Task.FromResult<TUser>(null);

            var appUser = new TUser(); //default(TUser);

            appUser.Id = userEntity.Id.ToString();
            appUser.UserId = userEntity.Id;
            appUser.Email = userEntity.Email;
            appUser.DisplayName = userEntity.DisplayName;
            return Task.FromResult(appUser);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
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
