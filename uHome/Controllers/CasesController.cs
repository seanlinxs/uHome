using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using uHome.Models;

namespace uHome.Controllers
{
    public class CasesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cases
        public async Task<ActionResult> Index()
        {
            var cases = db.Cases.Include(@ => @.CaseAssignment).Include(@ => @.CreatedBy);
            return View(await cases.ToListAsync());
        }

        // GET: Cases/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await db.Cases.FindAsync(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // GET: Cases/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.CaseAssignments, "CaseID", "ApplicationUserId");
            ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,Description,CreatedAt,State,ApplicationUserId")] Case @case)
        {
            if (ModelState.IsValid)
            {
                db.Cases.Add(@case);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.CaseAssignments, "CaseID", "ApplicationUserId", @case.ID);
            ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", @case.ApplicationUserId);
            return View(@case);
        }

        // GET: Cases/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await db.Cases.FindAsync(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.CaseAssignments, "CaseID", "ApplicationUserId", @case.ID);
            ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", @case.ApplicationUserId);
            return View(@case);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,Description,CreatedAt,State,ApplicationUserId")] Case @case)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@case).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.CaseAssignments, "CaseID", "ApplicationUserId", @case.ID);
            ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", @case.ApplicationUserId);
            return View(@case);
        }

        // GET: Cases/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await db.Cases.FindAsync(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Case @case = await db.Cases.FindAsync(id);
            db.Cases.Remove(@case);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
