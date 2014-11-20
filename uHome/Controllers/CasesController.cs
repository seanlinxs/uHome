using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using uHome.Models;
using System.Linq;
using uHome.Authorization;
using Thinktecture.IdentityModel.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace uHome.Controllers
{
    public class CasesController : BaseController
    {
        private int maxDisplayChars = int.Parse(ConfigurationManager.AppSettings["MaxDisplayChars"]);
        // GET: Cases
        [ResourceAuthorize(UhomeResources.Actions.List, UhomeResources.Case)]
        public ActionResult Index()
        {
            var caseGroups = new List<CaseGroupViewModel>();

            foreach (CaseState s in Enum.GetValues(typeof(CaseState)))
            {
                var cases = from c in Database.Cases
                            where c.State == s && c.ApplicationUserId == CurrentUser.Id
                            select new CaseListViewModel
                            {
                                ID = c.ID,
                                Title = c.Title,
                                CreatedBy = c.CreatedBy.UserName,
                                Description = c.Description,
                                DescriptionThumb = c.Description.Substring(0, maxDisplayChars),
                                Assignee = c.CaseAssignment == null ? "Unassigned" : c.CaseAssignment.Assignee.UserName,
                                CreatedAt = c.CreatedAt
                            };
                caseGroups.Add(new CaseGroupViewModel(s, cases));
            }

            return View(caseGroups);
        }

        // GET: Cases
        [ResourceAuthorize(UhomeResources.Actions.List, UhomeResources.Case)]
        public ActionResult List()
        {
            var caseGroups = new List<CaseGroupViewModel>();

            foreach (CaseState s in Enum.GetValues(typeof(CaseState)))
            {
                var cases = from c in Database.Cases
                            where c.State == s
                            select new CaseListViewModel
                            {
                                ID = c.ID,
                                Title = c.Title,
                                CreatedBy = c.CreatedBy.UserName,
                                Description = c.Description,
                                DescriptionThumb = c.Description.Substring(0, maxDisplayChars),
                                Assignee = c.CaseAssignment == null ? "Unassigned" : c.CaseAssignment.Assignee.UserName,
                                CreatedAt = c.CreatedAt
                            };
                caseGroups.Add(new CaseGroupViewModel(s, cases));
            }

            return View(caseGroups);
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

            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, id.ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            return View(@case);
        }

        // GET: Cases/AdminDetails/5
        public async Task<ActionResult> AdminDetails(int? id)
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

            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, id.ToString()))
            {
                return new HttpUnauthorizedResult();
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
            var now = System.DateTime.Now;

            if (ModelState.IsValid)
            {
                var @case = new Case
                {
                    Title = createCaseViewModel.Title,
                    Description = createCaseViewModel.Description,
                    CreatedAt = now,
                    CreatedBy = CurrentUser,
                    CaseAssignment = new CaseAssignment
                    {
                        // Default assign to system admin or manager
                        Assignee = await UserManager.FindByNameAsync("Administrator"),
                        AssignmentDate = now
                    }
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

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                return new HttpUnauthorizedResult();
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
