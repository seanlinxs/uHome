using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using uHome.Models;
using uHome.Jobs;
using System.IO;

namespace uHome
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            Scheduler.Start(); // Run scheduled jobs
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.Write(@"
<html>
  <head>
    <title>HTML Not Allowed</title>
  </head>
  <body>
    <h1>Oops!</h1>
    <p>I'm sorry, but HTML entry is not allowed on that page.</p>
    <p>
      Please make sure that your entries do not contain any angle brackets like &lt; or &gt;.
    </p>
    <p><a href='javascript:back()'>Go back</a></p>
  </body>
</html>
");
                Response.End();
            }
        }
    }
}
