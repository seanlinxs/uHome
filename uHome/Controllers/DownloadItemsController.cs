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
using System.IO;
using uHome.Authorization;
using Thinktecture.IdentityModel.Mvc;
using System.Data.Entity.Infrastructure;

namespace uHome.Controllers
{
    public class DownloadItemsController : BaseController
    {
        // GET: DownloadItems
        [ResourceAuthorize(UhomeResources.Actions.View, UhomeResources.DownloadItem)]
        public async Task<ActionResult> Index()
        {
            var downloadItems = new List<ListDownloadItemViewModel>();
            await Database.DownloadItems.ForEachAsync(di => downloadItems.Add(new ListDownloadItemViewModel(di)));

            return View(downloadItems);
        }

        // GET: DownloadItems/List
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.DownloadItem)]
        public async Task<ActionResult> List()
        {
            var downloadItems = new List<ListDownloadItemViewModel>();
            await Database.DownloadItems.ForEachAsync(di => downloadItems.Add(new ListDownloadItemViewModel(di)));

            return View(downloadItems);
        }

        // GET: DownloadItems/Download/5
        [ResourceAuthorize(UhomeResources.Actions.View, UhomeResources.DownloadItem)]
        public async Task<ActionResult> Download(int? id)
        {
            DownloadItem downloadItem = await Database.DownloadItems.FindAsync(id);

            return File(downloadItem.Path, MimeMapping.GetMimeMapping(downloadItem.FileName), downloadItem.FileName);
        }

        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.DownloadItem)]
        // GET: DownloadItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DownloadItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.DownloadItem)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,FileData")] CreateDownloadItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DownloadItem downloadItem = new DownloadItem();
                    downloadItem.Name = model.Name;
                    downloadItem.Description = model.Description;
                    var path = string.Format("{0}Uploads/{1}", AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
                    model.FileData.SaveAs(path);
                    downloadItem.Path = path;
                    downloadItem.FileName = Path.GetFileName(model.FileData.FileName);
                    downloadItem.Size = model.FileData.InputStream.Length;
                    Database.DownloadItems.Add(downloadItem);
                    await Database.SaveChangesAsync();

                    return RedirectToAction("List");
                }
                catch(DbUpdateException)
                {
                    ModelState.AddModelError("Name", Resources.Resources.MustBeUnique);
                }
            }

            return View(model);
        }

        // DELETE: DownloadItems/Delete/5
        [ResourceAuthorize(UhomeResources.Actions.Edit, UhomeResources.DownloadItem)]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            DownloadItem downloadItem = await Database.DownloadItems.FindAsync(id);

            Database.DownloadItems.Remove(downloadItem);
            await Database.SaveChangesAsync();

            return Json(new { Id = downloadItem.ID });
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
