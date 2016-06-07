using System.Web.Optimization;

namespace SFA.DAS.EmployerUsers.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/scripts/govuk-template.js"));

            bundles.Add(new StyleBundle("~/Content/stylesheets/bundled-css").Include(
                      "~/Content/stylesheets/govuk-template.css",
                      "~/Content/stylesheets/elements.css",
                      "~/Content/stylesheets/fonts.css",
                      "~/Content/stylesheets/das-controls.css",
                      "~/Content/stylesheets/users-main.css",
                      "~/Content/stylesheets/users-dashboard.css"));
        }
    }
}
