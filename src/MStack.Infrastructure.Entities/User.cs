using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Infrastructure.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public int LoginFailTimes { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Avatar { set; get; }
        public string Company { set; get; }
    }

    /// <summary>
    /// 第三方登录的存储信息
    /// </summary>
    public class UserLogin : BaseEntity
    {
        public Guid UserId { get; set; }
        //
        // 摘要:
        //     Provider for the linked login, i.e. Facebook, Google, etc.
        public string LoginProvider { get; set; }
        //
        // 摘要:
        //     User specific key for the login provider
        public string ProviderKey { get; set; }
    }

    public class UserClaim : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }
    }

}
