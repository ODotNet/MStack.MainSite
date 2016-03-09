using Microsoft.AspNet.Identity;
using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.MainSite.WebFramework.Authentication
{
    public class ApplicationUserRepository : MStackRepository<Guid>, IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>//, IUserLoginStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        private Microsoft.Owin.IOwinContext context;

        public ApplicationUserRepository(Microsoft.Owin.IOwinContext context)
        {
            // TODO: Complete member initialization
            this.context = context;
        }

        #region IUserStore
        public Task CreateAsync(ApplicationUser user)
        {
            var userEntity = new User()
            {
                Email = user.Email,
                HashedPassword = user.HashedPassword,
                DisplayName = user.DisplayName
            };
            return Task.Run(() => { Insert<User>(userEntity); });
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
            return Task.Run<ApplicationUser>(() => { return new ApplicationUser() { UserName = userName }; });
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            //throw new NotImplementedException();
            return Task.Run(() => { user.HashedPassword = passwordHash; });
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.Run<string>(() => { return user.HashedPassword; });
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.Run<bool>(() => false);
        }
        #endregion
    }
}
