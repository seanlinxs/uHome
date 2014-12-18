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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachment attachment = await Database.Attachments.FindAsync(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            if (!HttpContext.CheckAccess(UhomeResources.Actions.View, UhomeResources.Case, attachment.CaseID.ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            return File(attachment.FileStream, MimeMapping.GetMimeMapping(attachment.Name), attachment.Name);
        }

        // DELETE: Attachments/Delete/5
        [HttpDelete]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Attachment attachment = await Database.Attachments.FindAsync(id);
            
            if (attachment == null)
            {
                return HttpNotFound();
            }

            try
            {
                Database.Attachments.Remove(attachment);
                await Database.SaveChangesAsync();

                return Json(new { id = id});
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Gone);
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
