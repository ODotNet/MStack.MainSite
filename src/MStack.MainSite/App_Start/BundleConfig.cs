using System.Web;
using System.Web.Optimization;

namespace MStack.MainSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery.js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.pjax.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/fx.js").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/nprogress.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/site.js"));

            bundles.Add(new StyleBundle("~/Content/fx.css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/nprogress.css",
                      "~/Content/site.css"));
        }
    }
}
