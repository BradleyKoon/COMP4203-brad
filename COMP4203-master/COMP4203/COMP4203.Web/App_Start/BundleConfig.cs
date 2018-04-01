using System.Web.Optimization;

namespace COMP4203.Web
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			const string ANGULAR_APP_ROOT = "~/Scripts/";
			const string ANGULAR_VIRTUAL_BUNDLE_PATH = ANGULAR_APP_ROOT + "angularjs";

			var angularScriptBundle = new ScriptBundle(ANGULAR_VIRTUAL_BUNDLE_PATH)
				.Include(ANGULAR_APP_ROOT + "angular.js")
				.Include(ANGULAR_APP_ROOT + "angular-route.js")
				.Include(ANGULAR_APP_ROOT + "app.js")
				.IncludeDirectory(ANGULAR_APP_ROOT + "Controllers", "*.js", searchSubdirectories: false);

			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
					"~/Scripts/jquery-{version}.js",
					"~/Scripts/jquery.signalR-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			bundles.Add(angularScriptBundle);

			bundles.Add(new ScriptBundle("~/bundles/ui-bootstrap").Include(
				"~/Scripts/ui-bootstrap-tpls.js"));

			bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
				"~/Scripts/highcharts.src.js"));

			bundles.Add(new ScriptBundle("~/bundles/highcharts-ng").Include(
				"~/Scripts/highcharts-ng.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/highcharts.css",
					  "~/Content/site.css"));

			BundleTable.EnableOptimizations = false;
		}
	}
}
