using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MStack.MainSite.Models
{
    public class UserModel
    {
        [Required]
        [System.Web.Mvc.HiddenInput]
        public Guid Id { set; get; }
        [ReadOnly(true)]
        [Display(Name ="账号")]
        public string UserName { get; set; }
        [ReadOnly(true)]
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "昵称")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string DisplayName { get; set; }
        [Display(Name = "头像")]
        public string Avatar { set; get; }
        [Display(Name = "公司")]
        public string Company { set; get; }
        [System.Web.Mvc.HiddenInput]
        public string AvatarState { set; get; }
    }
    
}