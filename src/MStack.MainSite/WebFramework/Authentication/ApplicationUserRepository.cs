using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.MainSite.WebFramework.Authentication
{
  public  class ApplicationUserRepository: IUserStore<ApplicationUser>//, IUserLoginStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
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
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return null;
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return null;
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
