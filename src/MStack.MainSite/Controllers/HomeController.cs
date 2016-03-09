using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        [Authorize]
        public ActionResult BookList()
        {
            return View();
        }

        #region DbHelper
        public ActionResult DbUpdate(bool isRun = false)
        {
            return Content(string.Format("<pre>{0}</pre>", UpdateDbSql(isRun)));
        }

        private static string UpdateDbSql(bool isRun)
        {
            var sbSql = new StringBuilder();

            var configuration = new Configuration();
            configuration.Configure();

            var session = configuration.BuildSessionFactory().OpenSession();

            using (var tx = session.BeginTransaction())
            {
                var tempFileName = Path.GetTempFileName();
                try
                {
                    using (var str = new StreamWriter(tempFileName))
                    {
                        new SchemaUpdate(configuration).Execute(action =>
                        {
                            if (action.IndexOf("foreign key") < 0)
                                sbSql.AppendLine(action + ";");
                        }, isRun);
                    }
                }
                finally
                {
                    if (System.IO.File.Exists(tempFileName))
                    {
                        System.IO.File.Delete(tempFileName);
                    }
                }

                tx.Commit();
            }
            return sbSql.ToString();
        }

        public ActionResult DbCreate(bool isRun = false)
        {
            return Content(string.Format("<pre>{0}</pre>", GenerateDbSql(isRun)));
        }

        private static string GenerateDbSql(bool isRun)
        {
            var sbSql = new StringBuilder();

            var configuration = new Configuration();
            configuration.Configure();

            var session = configuration.BuildSessionFactory().OpenSession();

            using (var tx = session.BeginTransaction())
            {
                var tempFileName = Path.GetTempFileName();
                try
                {
                    using (var str = new StreamWriter(tempFileName))
                    {
                        new SchemaExport(configuration).Create(action =>
                        {
                            if (action.IndexOf("foreign key") < 0)
                                sbSql.AppendLine(action + ";");
                        }, isRun);
                    }
                }
                finally
                {
                    if (System.IO.File.Exists(tempFileName))
                    {
                        System.IO.File.Delete(tempFileName);
                    }
                }

                tx.Commit();
            }
            return sbSql.ToString();
        }

        public ActionResult DbDrop(bool isRun = false)
        {
            return Content(string.Format("<pre>{0}</pre>", DropDb(isRun)));
        }

        private static string DropDb(bool isRun)
        {
            var sbSql = new StringBuilder();

            var configuration = new Configuration();
            configuration.Configure();

            var session = configuration.BuildSessionFactory().OpenSession();

            using (var tx = session.BeginTransaction())
            {
                var tempFileName = Path.GetTempFileName();
                try
                {
                    using (var str = new StreamWriter(tempFileName))
                    {
                        new SchemaExport(configuration).Execute(action =>
                        {
                            ////if (action.IndexOf("foreign key") < 0)
                            sbSql.AppendLine(action + ";");
                        }, isRun, true);
                    }
                }
                finally
                {
                    if (System.IO.File.Exists(tempFileName))
                    {
                        System.IO.File.Delete(tempFileName);
                    }
                }

                tx.Commit();
            }
            return sbSql.ToString();
        }
        #endregion
    }
}