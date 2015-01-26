using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using uHome.Helpers;
using uHome.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace uHome.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            IEnumerable<EventViewModel> events = Database.Events.ToList().Select(e => new EventViewModel(e));
            IEnumerable<VideoClip> videoClips = Database.VideoClips.ToList();
            HomeIndexViewModel model = new HomeIndexViewModel(events, videoClips);

            return View(model);
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

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];

            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);

            return Redirect(Request.UrlReferrer.ToString());
        } 
    }
}