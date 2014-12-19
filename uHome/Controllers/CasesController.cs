﻿using System.Data.Entity;
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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using uHome.Extensions;
using System.Threading;

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
                                Title = c.Title ?? "N/A",
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

            foreach (CaseState s in new CaseState[] { CaseState.NEW, CaseState.ACTIVE })
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
        public async Task<ActionResult> Create([Bind(Include = "Title, Description")] CreateCaseViewModel createCaseViewModel)
        {
            var now = System.DateTime.Now;

            if (ModelState.IsValid)
            {
                var @case = new Case
                {
                    Title = createCaseViewModel.Title,
                    Description = createCaseViewModel.Description,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = CurrentUser,
                    State = CaseState.NEW,
                    CaseAssignment = new CaseAssignment
                    {
                        // Default assign to system admin or manager
                        Assignee = await UserManager.FindByNameAsync("Administrator"),
                        AssignmentDate = now
                    }
                };

                Database.Cases.Add(@case);
                await Database.SaveChangesAsync();

                return RedirectToAction("Edit", new { id = @case.ID });
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

            var model = new EditCaseViewModel(@case);
            ViewBag.Assignee = new SelectList(UserManager.GetAssigneeSet(Database), "Id", "UserName", @case.CaseAssignment.ApplicationUserId);
            
            return View(model);
        }

        // POST: Cases/AddFile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<ActionResult> AddFile(int? id)
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

            var error = @case.AddFiles(Request.Files);

            if (error == null)
            {
                @case.UpdatedAt = System.DateTime.Now;
                Database.Entry(@case).State = EntityState.Modified;
                await Database.SaveChangesAsync();

                // Build an ajax response data for uploadify
                return Json(new { success = true, updatedAt = @case.UpdatedAt.ToString() });
            }
            else // Exceed maximum storage size of case, cannot add more file
            {
                return Json(new { success = false, errMsg = string.Format("Could not save {0}, exceed quota limit(100MB)", error) });
            }
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
