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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using uHome.Extensions;
using System.Threading;
using System.IO;

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
                    OldState = CaseState.CLOSED,
                    CaseAssignment = new CaseAssignment
                    {
                        // Default assign to system admin or manager
                        Assignee = await UserManager.FindByNameAsync("Administrator"),
                        AssignmentDate = now
                    }
                };

                Database.Cases.Add(@case);
                await Database.SaveChangesAsync();
                @case.Title = string.Format("CASE-{0}: {1}", @case.ID, @case.Title);
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
            
            return View(model);
        }

        // POST: Cases/AddFile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            var file = Request.Files[0];
            var fileName = Path.GetFileName(file.FileName);

            try
            {
                var attachment = @case.AddFile(file);

                if (attachment != null)
                {
                    @case.UpdatedAt = System.DateTime.Now;
                    Database.Entry(@case).State = EntityState.Modified;
                    await Database.SaveChangesAsync();

                    // Build an ajax response data for uploadify
                    return Json(new { success = true, updatedAt = @case.UpdatedAt.ToString(),
                        attachmentRow = this.RenderPartialViewToString("_EditAttachmentPartial", new AttachmentViewModel(attachment)) });
                }
                else // Exceed maximum storage size of case, cannot add more file
                {
                    return Json(new
                    {
                        success = false,
                        error = string.Format(Resources.Resources.UploadedFailed, fileName, Case.MAX_STORAGE_SIZE / 1024 / 1024)
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = string.Format("Save {0} failed: {1}", fileName, e.Message) });
            }
        }

        // GET: Cases/AdminEdit/5
        [ResourceAuthorize(UhomeResources.Actions.AdminEdit, UhomeResources.Case)]
        public async Task<ActionResult> AdminEdit(int? id)
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

            var model = new EditCaseViewModel(@case);
            ViewBag.Assignee = new SelectList(UserManager.GetAssigneeSet(), "Id", "UserName", @case.CaseAssignment.ApplicationUserId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Close(int? id)
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

            try
            {
                @case.OldState = @case.State;
                @case.State = CaseState.CLOSED;
                @case.UpdatedAt = System.DateTime.Now;
                await Database.SaveChangesAsync();

                return Json(new {
                    success = true,
                    updatedAt = @case.UpdatedAt.ToString(),
                    state = @case.State.ToString(),
                    actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new EditCaseViewModel(@case))
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reopen(int? id)
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

            try
            {
                @case.State = @case.OldState;
                @case.UpdatedAt = System.DateTime.Now;
                await Database.SaveChangesAsync();

                return Json(new {
                    success = true,
                    updatedAt = @case.UpdatedAt.ToString(),
                    state = @case.State.ToString(),
                    actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new EditCaseViewModel(@case))
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
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
