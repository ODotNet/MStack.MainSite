using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult KeepAlive()
        {
            return Json(new { alive = true, inProcessingRequest = ApplicationStaticits.ProcessingRequestCount, Now = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() }, JsonRequestBehavior.AllowGet);
            //return Content( string.Format("Alive, Total InProcessingRequest:{0}, DateTime:{1} ", ApplicationStaticits.ProcessingRequestCount, DateTime.Now));
        }


        public ActionResult BookList()
        {
            return View();
        }
    }
}