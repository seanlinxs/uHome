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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace uHome
{
    public class VideoClipsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager userManager;
        
        public VideoClipsController()
        {
           userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        // GET: VideoClips
        public async Task<ActionResult> Index()
        {
            var videoClips = db.VideoClips.Include(v => v.UploadedBy);
            return View(await videoClips.ToListAsync());
        }

        // GET: VideoClips/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoClip videoClip = await db.VideoClips.FindAsync(id);
            if (videoClip == null)
            {
                return HttpNotFound();
            }
            return View(videoClip);
        }

        // GET: VideoClips/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: VideoClips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateVideoClipViewModel model)
        {
            if (ModelState.IsValid)
            {
                VideoClip videoClip = new VideoClip(model);
                videoClip.UploadedBy = userManager.FindById(User.Identity.GetUserId());
                db.VideoClips.Add(videoClip);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: VideoClips/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoClip videoClip = await db.VideoClips.FindAsync(id);
            if (videoClip == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", videoClip.ApplicationUserId);
            return View(videoClip);
        }

        // POST: VideoClips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Path,UploadedAt,ApplicationUserId")] VideoClip videoClip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(videoClip).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", videoClip.ApplicationUserId);
            return View(videoClip);
        }

        // GET: VideoClips/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoClip videoClip = await db.VideoClips.FindAsync(id);
            if (videoClip == null)
            {
                return HttpNotFound();
            }
            return View(videoClip);
        }

        // POST: VideoClips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            VideoClip videoClip = await db.VideoClips.FindAsync(id);
            db.VideoClips.Remove(videoClip);
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
