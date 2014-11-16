using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using uHome.Models;
using System.Linq;
using uHome.Authorization;
using Thinktecture.IdentityModel.Mvc;

namespace uHome.Controllers
{
    public class CasesController : BaseController
    {
        // GET: Cases
        [ResourceAuthorize(UhomeResources.CaseActions.View, UhomeResources.Case)]
        public ActionResult Index()
        {
            var cases = from c in CurrentUser.Cases
                        select new ListCaseViewModel(c);
            return View(cases);
        }

        // GET: Cases/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await Database.Cases.FindAsync(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // GET: Cases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,Description")] CreateCaseViewModel createCaseViewModel)
        {
            if (ModelState.IsValid)
            {
                var @case = new Case
                {
                    Title = createCaseViewModel.Title,
                    Description = createCaseViewModel.Description,
                    CreatedAt = System.DateTime.Now,
                    CreatedBy = CurrentUser
                };
                Database.Cases.Add(@case);
                await Database.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Cases/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await Database.Cases.FindAsync(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(Database.CaseAssignments, "CaseID", "ApplicationUserId", @case.ID);
            ViewBag.ApplicationUserId = new SelectList(Database.Users, "Id", "Email", @case.ApplicationUserId);
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
                Database.Entry(@case).State = EntityState.Modified;
                await Database.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(Database.CaseAssignments, "CaseID", "ApplicationUserId", @case.ID);
            ViewBag.ApplicationUserId = new SelectList(Database.Users, "Id", "Email", @case.ApplicationUserId);
            return View(@case);
        }

        // GET: Cases/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = await Database.Cases.FindAsync(id);
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
            Case @case = await Database.Cases.FindAsync(id);
            Database.Cases.Remove(@case);
            await Database.SaveChangesAsync();
            return RedirectToAction("Index");
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
