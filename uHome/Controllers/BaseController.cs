﻿using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using uHome.Helpers;
using uHome.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace uHome.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
        }
        protected ApplicationUserManager userManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager roleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        protected ApplicationSignInManager signInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        protected ApplicationUser currentUser
        {
            get
            {
                return userManager.FindById(User.Identity.GetUserId());
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
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }
    }
}