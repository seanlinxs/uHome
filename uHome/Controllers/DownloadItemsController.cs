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

namespace uHome.Controllers
{
    public class DownloadItemsController : BaseController
    {
        // GET: DownloadItems
        public async Task<ActionResult> Index()
        {
            return View(await Database.DownloadItems.ToListAsync());
        }

        // GET: DownloadItems/List
        public async Task<ActionResult> List()
        {
            var downloadItems = new List<ListDownloadItemViewModel>();
            await Database.DownloadItems.ForEachAsync(di => downloadItems.Add(new ListDownloadItemViewModel(di)));

            return View(downloadItems);
        }

        // GET: DownloadItems/Download/5
        public async Task<ActionResult> Download(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DownloadItem downloadItem = await Database.DownloadItems.FindAsync(id);

            if (downloadItem == null)
            {
                return HttpNotFound();
            }

            return File(downloadItem.Path, MimeMapping.GetMimeMapping(downloadItem.FileName), downloadItem.FileName);
        }

        // GET: DownloadItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DownloadItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,FileData")] CreateDownloadItemViewModel model)
        {
            if (ModelState.IsValid)
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

            return View(model);
        }

        // GET: DownloadItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadItem downloadItem = await Database.DownloadItems.FindAsync(id);
            if (downloadItem == null)
            {
                return HttpNotFound();
            }
            return View(downloadItem);
        }

        // POST: DownloadItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Description,Path")] DownloadItem downloadItem)
        {
            if (ModelState.IsValid)
            {
                Database.Entry(downloadItem).State = EntityState.Modified;
                await Database.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(downloadItem);
        }

        // POST: DownloadItems/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            DownloadItem downloadItem = await Database.DownloadItems.FindAsync(id);
            
            if (downloadItem == null)
            {
                return HttpNotFound();
            }

            try
            {
                Database.DownloadItems.Remove(downloadItem);
                await Database.SaveChangesAsync();

                return Json(new { success = true, Id = downloadItem.ID });
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
