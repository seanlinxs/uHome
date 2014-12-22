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
    public class DownloadItemsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DownloadItems
        public async Task<ActionResult> Index()
        {
            return View(await db.DownloadItems.ToListAsync());
        }

        // GET: DownloadItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadItem downloadItem = await db.DownloadItems.FindAsync(id);
            if (downloadItem == null)
            {
                return HttpNotFound();
            }
            return View(downloadItem);
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
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description,Path")] DownloadItem downloadItem)
        {
            if (ModelState.IsValid)
            {
                db.DownloadItems.Add(downloadItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(downloadItem);
        }

        // GET: DownloadItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadItem downloadItem = await db.DownloadItems.FindAsync(id);
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
                db.Entry(downloadItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(downloadItem);
        }

        // GET: DownloadItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadItem downloadItem = await db.DownloadItems.FindAsync(id);
            if (downloadItem == null)
            {
                return HttpNotFound();
            }
            return View(downloadItem);
        }

        // POST: DownloadItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DownloadItem downloadItem = await db.DownloadItems.FindAsync(id);
            db.DownloadItems.Remove(downloadItem);
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
