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
using uHome.Controllers;
using uHome.Authorization;

namespace uHome
{
    public class AttachmentsController : BaseController
    {
        // GET: Attachments/Download/5
        public async Task<ActionResult> Download(int? id)
        {
            Attachment attachment = await Database.Attachments.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, attachment.CaseID.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            return File(attachment.Path, MimeMapping.GetMimeMapping(attachment.Name), attachment.Name);
        }

        // DELETE: Attachments/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            Attachment attachment = await Database.Attachments.FindAsync(id);

            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, attachment.CaseID.ToString()))
            {
                throw new Exception(Resources.Resources.PermissionDenied);
            }

            Case @case = await Database.Cases.FindAsync(attachment.CaseID);
            @case.UpdatedAt = System.DateTime.Now;

            @case.DelAttachment(attachment);
            Database.Attachments.Remove(attachment);
            Database.Entry(@case).State = EntityState.Modified;
            await Database.SaveChangesAsync();

            return Json(new { success = true, id = id, updatedAt = @case.UpdatedAt.ToString() });
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
