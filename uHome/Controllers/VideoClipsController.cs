using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using uHome.Models;
using Thinktecture.IdentityModel.Mvc;
using uHome.Authorization;

namespace uHome.Controllers
{
    public class VideoClipsController : BaseController
    {
        // GET: VideoClips
        [ResourceAuthorize(UhomeResources.VideoClipActions.View, UhomeResources.VideoClip)]
        public async Task<ActionResult> Index()
        {
            var videoClips = Database.VideoClips.Include(v => v.UploadedBy);
            return View(await videoClips.ToListAsync());
        }

        // GET: VideoClips/List
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> List()
        {
            var videoClips = Database.VideoClips.Include(v => v.UploadedBy);
            return View(await videoClips.ToListAsync());
        }

        // GET: VideoClips/Details/5
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VideoClip videoClip = await Database.VideoClips.FindAsync(id);

            if (videoClip == null)
            {
                return HttpNotFound();
            }

            return View(videoClip);
        }

        // GET: VideoClips/Create
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: VideoClips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> Create([Bind(Include = "Name, Description, Path")]VideoClipViewModel model)
        {
            if (ModelState.IsValid)
            {
                VideoClip videoClip = new VideoClip(model);
                videoClip.UploadedBy = CurrentUser;
                Database.VideoClips.Add(videoClip);
                await Database.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: VideoClips/Edit/5
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VideoClip videoClip = await Database.VideoClips.FindAsync(id);
            
            if (videoClip == null)
            {
                return HttpNotFound();
            }


            VideoClipViewModel videoClipViewModel = new VideoClipViewModel {
                Id = videoClip.ID,
                Name = videoClip.Name,
                Description = videoClip.Description,
                Path = videoClip.Path
            };

            return View(videoClipViewModel);
        }

        // POST: VideoClips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Name, Description, Path")]VideoClipViewModel model)
        {
            if (ModelState.IsValid)
            {
                VideoClip videoClip = await Database.VideoClips.FindAsync(model.Id);

                if (videoClip == null)
                {
                    return HttpNotFound();
                }

                Database.Entry(videoClip).State = EntityState.Modified;
                videoClip.Name = model.Name;
                videoClip.Description = model.Description;
                videoClip.Path = model.Path;
                await Database.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: VideoClips/Delete/5
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VideoClip videoClip = await Database.VideoClips.FindAsync(id);
            
            if (videoClip == null)
            {
                return HttpNotFound();
            }

            return View(videoClip);
        }

        // POST: VideoClips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(UhomeResources.VideoClipActions.Edit, UhomeResources.VideoClip)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            VideoClip videoClip = await Database.VideoClips.FindAsync(id);
            Database.VideoClips.Remove(videoClip);
            await Database.SaveChangesAsync();
            return RedirectToAction("Index");
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
