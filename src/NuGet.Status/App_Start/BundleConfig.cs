using System.Web.Optimization;

namespace NuGet.Status
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/gallery/css/site.min.css")
                .Include(
                    "~/Content/gallery/css/bootstrap.css",
                    "~/Content/gallery/css/bootstrap-theme.css",
                    "~/Content/gallery/css/fabric.css"));

            bundles.Add(new ScriptBundle("~/Scripts/gallery/site.min.js")
                .Include(
                    "~/Scripts/gallery/jquery-3.4.1.js",
                    "~/Scripts/gallery/jquery.validate-1.16.0.js",
                    "~/Scripts/gallery/jquery.validate.unobtrusive-3.2.6.js",
                    "~/Scripts/gallery/knockout-3.4.2.js",
                    "~/Scripts/gallery/bootstrap.js",
                    "~/Scripts/gallery/moment-2.18.1.js",
                    "~/Scripts/gallery/common.js",
                    "~/Scripts/gallery/page-status.js"));
        }
    }
}