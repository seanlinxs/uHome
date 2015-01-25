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
using System.Net.Mime;

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
            // Only wrap exception for ajax request, use default machenism for non-ajax request
            if (new HttpRequestWrapper(Request).IsAjaxRequest())
            {
                Exception ex = Server.GetLastError();
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500; // Capture our own exceptions and mark as internal server error
                Response.Clear();
                Response.ContentType = MediaTypeNames.Text.Plain;
                Response.Write(ex.Message);
                Response.End();
            }
        }
    }
}
