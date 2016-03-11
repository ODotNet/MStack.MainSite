using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
            this.DataContext = new MStackRepository<Guid>(NHSessionFactory.OpenSession());
        }
        // GET: User
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("UserList");
        }

        public ActionResult UserList()
        {
            var user = DataContext.GetQuery<User>().ToList();
            return View(user);
        }

        public ActionResult UserLoginList(Guid userId)
        {
            var userLoginList = DataContext.GetQuery<UserLogin>().Where(x => x.UserId == userId).ToList();
            return View(userLoginList);
        }

        public MStackRepository<Guid> DataContext { get; set; }
    }
}