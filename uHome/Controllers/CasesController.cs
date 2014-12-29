using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using uHome.Models;
using System.Linq;
using uHome.Authorization;
using Thinktecture.IdentityModel.Mvc;
using System;
using System.Collections.Generic;
using uHome.Extensions;
using System.IO;
using Newtonsoft.Json;

namespace uHome.Controllers
{
    public class CasesController : BaseController
    {
        // GET: Cases
        [ResourceAuthorize(UhomeResources.Actions.List, UhomeResources.Case)]
        public ActionResult Index()
        {
            var caseGroups = new List<CaseGroupViewModel>();

            foreach (CaseState s in new CaseState[] { CaseState.NEW, CaseState.ASSIGNED, CaseState.ACTIVE, CaseState.CLOSED })
            {
                var cases = Database.Cases
                    .Where(c => c.ApplicationUserId == CurrentUser.Id)
                    .Where(c => c.State == s)
                    .ToList();
                var models = cases.Select(c => new CaseListViewModel(c));
                caseGroups.Add(new CaseGroupViewModel(s, models));
            }

            return View(caseGroups);
        }

        // GET: Cases
        [ResourceAuthorize(UhomeResources.Actions.List, UhomeResources.Case)]
        public ActionResult List()
        {
            var caseGroups = new List<CaseGroupViewModel>();

            foreach (CaseState s in new CaseState[] { CaseState.NEW, CaseState.ASSIGNED, CaseState.ACTIVE })
            {
                var cases = Database.Cases.Where(c => c.State == s).ToList();
                var models = cases.Select(c => new CaseListViewModel(c));
                caseGroups.Add(new CaseGroupViewModel(s, models));
            }

            return View(caseGroups);
        }

        // GET: Cases
        [ResourceAuthorize(UhomeResources.Actions.List, UhomeResources.Case)]
        public ActionResult StaffList()
        {
            var caseGroups = new List<CaseGroupViewModel>();

            foreach (CaseState s in new CaseState[] { CaseState.ASSIGNED, CaseState.ACTIVE, CaseState.CLOSED })
            {
                var cases = Database.Cases
                    .Where(c => c.CaseAssignment.ApplicationUserId == CurrentUser.Id)
                    .Where(c => c.State == s)
                    .ToList();
                var models = cases.Select(c => new CaseListViewModel(c));
                caseGroups.Add(new CaseGroupViewModel(s, models));
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
                    //CaseAssignment = new CaseAssignment
                    //{
                    //    // Default assign to system admin or manager
                    //    Assignee = await UserManager.FindByNameAsync("Administrator"),
                    //    AssignmentDate = now
                    //}
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
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            var model = new EditCaseViewModel(@case);

            return View(model);
        }

        // GET: Cases/StaffEdit/5
        public async Task<ActionResult> StaffEdit(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.StaffEdit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            var model = new StaffEditCaseViewModel(@case);

            return View(model);
        }

        // POST: Cases/AddFile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddFile(int? id, HttpPostedFileBase file)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            var attachment = @case.AddFile(file);

            @case.UpdatedAt = System.DateTime.Now;
            Database.Entry(@case).State = EntityState.Modified;
            await Database.SaveChangesAsync();

            // Build an ajax response data for uploadify
            return Json(new
            {
                updatedAt = @case.UpdatedAt.ToString(),
                newAttachmentRow = this.RenderPartialViewToString("_EditAttachmentPartial", new AttachmentViewModel(attachment))
            });
        }

        // POST: Cases/AddFiles/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // This is a fallback to upload files if browser does not support HTML 5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddFiles(int? id, ICollection<HttpPostedFileBase> UploadFiles)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            if (UploadFiles != null)
            {
                foreach (var file in UploadFiles)
                {
                    @case.AddFile(file);
                }
            }

            @case.UpdatedAt = System.DateTime.Now;
            Database.Entry(@case).State = EntityState.Modified;
            await Database.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = @case.ID });
        }

        // GET: Cases/AdminEdit/5
        [ResourceAuthorize(UhomeResources.Actions.AdminEdit, UhomeResources.Case)]
        public async Task<ActionResult> AdminEdit(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);
            var model = new EditCaseViewModel(@case);
            var AssigneeCadidates = UserManager.GetAssigneeCadidates();
            var AssigneeList = new Dictionary<string, string>();
            AssigneeList.Add("unassigned", "Unassigned");

            foreach (var a in AssigneeCadidates)
            {
                int total = Database.CaseAssignments.Where(ca => ca.ApplicationUserId == a.Id).Count();
                int active = Database.Cases.Where(c => c.State == CaseState.ACTIVE).Where(c => c.CaseAssignment.ApplicationUserId == a.Id).Count();
                AssigneeList.Add(a.Id, string.Format("{0} ({1}/{2})", a.UserName, total, active));
            }

            if (@case.CaseAssignment == null)
            {
                AssigneeList.Add("selected", "unassigned");
            }
            else
            {
                AssigneeList.Add("selected", @case.CaseAssignment.ApplicationUserId);
            }

            ViewBag.AssigneeSelectList = JsonConvert.SerializeObject(AssigneeList);
            logger.Debug(ViewBag.AssigneeSelectList);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Close(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            @case.OldState = @case.State;
            @case.State = CaseState.CLOSED;
            @case.UpdatedAt = System.DateTime.Now;
            await Database.SaveChangesAsync();

            return Json(new
            {
                updatedAt = @case.UpdatedAt.ToString(),
                state = @case.State.ToString(),
                actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new EditCaseViewModel(@case))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reopen(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            @case.State = @case.OldState;
            @case.UpdatedAt = System.DateTime.Now;
            await Database.SaveChangesAsync();

            return Json(new
            {
                updatedAt = @case.UpdatedAt.ToString(),
                state = @case.State.ToString(),
                actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new EditCaseViewModel(@case))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Assign(int? id, string user_id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.Edit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            if (user_id == "unassigned")
            {
                Database.CaseAssignments.Remove(@case.CaseAssignment);
                @case.State = CaseState.NEW;
                @case.UpdatedAt = System.DateTime.Now;
                await Database.SaveChangesAsync();

                return "Unassigned";
            }
            else
            {
                if (@case.CaseAssignment == null)
                {
                    @case.CaseAssignment = new CaseAssignment();
                }

                @case.CaseAssignment.ApplicationUserId = user_id;
                @case.CaseAssignment.AssignmentDate = System.DateTime.Now;
                @case.State = CaseState.ASSIGNED;
                @case.UpdatedAt = System.DateTime.Now;
                await Database.SaveChangesAsync();

                return (await UserManager.FindByIdAsync(user_id)).UserName;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.StaffEdit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            @case.OldState = @case.State;
            @case.State = CaseState.ACTIVE;
            @case.UpdatedAt = System.DateTime.Now;
            await Database.SaveChangesAsync();

            return Json(new
            {
                updatedAt = @case.UpdatedAt.ToString(),
                state = @case.State.ToString(),
                actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new StaffEditCaseViewModel(@case))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Stop(int? id)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.StaffEdit, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            @case.OldState = @case.State;
            @case.State = CaseState.ASSIGNED;
            @case.UpdatedAt = System.DateTime.Now;
            await Database.SaveChangesAsync();

            return Json(new
            {
                updatedAt = @case.UpdatedAt.ToString(),
                state = @case.State.ToString(),
                actionLink = this.RenderPartialViewToString("_ChangeStateLinkPartial", new StaffEditCaseViewModel(@case))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(int? id, string value)
        {
            Case @case = await Database.Cases.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, id.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            var commentViewModel = @case.AddComment(value, CurrentUser);
            @case.UpdatedAt = System.DateTime.Now;
            await Database.SaveChangesAsync();

            return Json(new { newCommentRow = this.RenderPartialViewToString("_EditCommentPartial", commentViewModel) });
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
