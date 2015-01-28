using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using uHome.Helpers;
using uHome.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Globalization;

namespace uHome.Controllers
{
    public class BaseController : Controller
    {
        public readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
        public ApplicationDbContext Database
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];

            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                    Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                    null;

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures
            CultureInfo culture = new CultureInfo(cultureName);
            
            if (cultureName.StartsWith("zh"))
            {
                culture.DateTimeFormat.ShortDatePattern = "yyyy'年'M'月'd'日' dddd";
                culture.DateTimeFormat.LongTimePattern = "tt h:mm";
            }
            else
            {
                culture.DateTimeFormat.ShortDatePattern = "dddd, d MMMM yyyy";
                culture.DateTimeFormat.LongTimePattern = "h:mm tt";
            }
            
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Request.IsAuthenticated)
            {
                filterContext.Controller.ViewBag.EmailConfirmed = CurrentUser.EmailConfirmed;
            }

            base.OnResultExecuting(filterContext);
        }
    }
}