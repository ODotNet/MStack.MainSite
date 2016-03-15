using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    public class ManageController : Controller
    {
        private MStackRepository<Guid> DataContext { get; set; }
        public ManageController()
        {
            this.DataContext = new MStackRepository<Guid>(NHSessionFactory.OpenSession());
        }
        // GET: Manage
        public ActionResult Index()
        {
            var userName = UserIdentity.Name;
            var user = this.DataContext.GetQuery<User>().SingleOrDefault(x => x.UserName == userName);
            return View(user);
        }

        protected ClaimsIdentity UserIdentity
        {
            get
            {
                return User.Identity as ClaimsIdentity;
            }
        }
    }
}