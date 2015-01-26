using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;
using uHome.Authorization;
using uHome.Models;

namespace uHome.Controllers
{
    public class EventsController : BaseController
    {
        // GET: Events
        public ActionResult Index()
        {
            return View(Database.Events.ToList().Select(e => new EventViewModel(e)));
        }

        // GET: Events
        [ResourceAuthorize(UhomeResources.EventActions.Edit, UhomeResources.Event)]
        public ActionResult List()
        {
            return View(Database.Events.ToList().Select(e => new EventViewModel(e)));
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            Event e = Database.Events.Find(id);
            EventViewModel model = new EventViewModel(e);
            model.Enrollments = e.Enrollments.Select(x => new ListEnrollmentViewModel(x));

            return View(model);
        }

        // GET: Events/AdminDetails/5
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.Event)]
        public ActionResult AdminDetails(int? id)
        {
            Event e = Database.Events.Find(id);
            EventViewModel model = new EventViewModel(e);
            model.Enrollments = e.Enrollments.Select(x => new ListEnrollmentViewModel(x));

            return View(model);
        }

        // GET: Events/Create
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.Event)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.Event)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,OpenAt,Address,Poster")] CreateEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = new Event(model);
                Database.Events.Add(e);
                Database.SaveChanges();

                return RedirectToAction("List");
            }
            else
                return View(model);
        }

        // DELETE: Events/Delete/5
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.Event)]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Event @event = Database.Events.Find(id);
            Database.Events.Remove(@event);
            Database.SaveChanges();

            return Json(new { Id = @event.ID });
        }

        // GET: Events/Join/5
        public ActionResult Join(int id)
        {
            Event e = Database.Events.Find(id);
            ViewBag.Event = e;

            return View();
        }

        // POST: Events/Join/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join(int id,
            [Bind(Include = "Email, Number, FullName, Country, State, City, Address")]CreateEnrollmentViewModel model)
        {
            Event e = Database.Events.Find(id);

            if (ModelState.IsValid)
            {
                Enrollment enrollment = new Enrollment(model);
                e.AddEnrollment(enrollment);
                Database.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
