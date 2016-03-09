using MStack.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MStack.MainSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        const string NO_WWW_HOST = "mstack.club";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            System.Threading.Interlocked.Increment(ref ApplicationStaticits.ProcessingRequestCount);
            if (Request.Url.Host.EndsWith(NO_WWW_HOST, StringComparison.OrdinalIgnoreCase) && Request.Url.Host.Equals(NO_WWW_HOST, StringComparison.OrdinalIgnoreCase))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Host = string.Format("www.{0}", Request.Url.Host);
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {

            if (System.Web.HttpContext.Current.Response.StatusCode >= 500)
                SessionManager.Rollback();
            else
                SessionManager.Commit();
            System.Threading.Interlocked.Decrement(ref ApplicationStaticits.ProcessingRequestCount);
        }
    }

    public class ApplicationStaticits
    {
        public static int ProcessingRequestCount = 0;
    }
}
