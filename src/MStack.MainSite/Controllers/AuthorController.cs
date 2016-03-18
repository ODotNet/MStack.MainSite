using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    public class AuthorController : Controller
    {
        private MStackRepository<Guid> DataContext;
        public AuthorController()
        {
            this.DataContext = new MStackRepository<Guid>(NHSessionFactory.OpenSession());
        }

        // GET: Author
        public ActionResult Index()
        {
            var user = DataContext.GetQuery<User>().ToList();
            return View(user);
        }
    }
}