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
            return View(new EventViewModel(e));
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

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = Database.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,OpenAt,Address,Poster")] Event @event)
        {
            if (ModelState.IsValid)
            {
                Database.Entry(@event).State = EntityState.Modified;
                Database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
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
