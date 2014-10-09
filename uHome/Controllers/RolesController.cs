using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using uHome.Models;

namespace uHome.Controllers
{
    public class RolesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<RoleViewModel> rolesList = new List<RoleViewModel>();

            foreach (ApplicationRole role in db.Roles)
            {
                var roleModel = new RoleViewModel(role);
                rolesList.Add(roleModel);
            }

            return View(rolesList);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "RoleName, Description")]RoleViewModel model)
        {
            string message = "That role name has already been used";

            if (ModelState.IsValid)
            {
                ApplicationRole role = new ApplicationRole(model.RoleName, model.Description);
                ApplicationRoleManager roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));

                if (db.RoleExists(roleManager, model.RoleName))
                    return View(message);
                else
                {
                    db.CreateRole(roleManager, model.RoleName, model.Description);
                    return RedirectToAction("Index", "Roles");
                }
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            // It's actually the Role.Name tucked into the id param:
            ApplicationRole role = db.Roles.First(r => r.Name == id) as ApplicationRole;
            EditRoleViewModel editRoleModel = new EditRoleViewModel(role);

            return View(editRoleModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "RoleName, OriginalRoleName, Description")]EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = db.Roles.First(r => r.Name == model.OriginalRoleName) as ApplicationRole;
                role.Name = model.RoleName;
                role.Description = model.Description;
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationRole role = db.Roles.First(r => r.Name == id) as ApplicationRole;
            RoleViewModel model = new RoleViewModel(role);

            if (role == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationRole role = db.Roles.First(r => r.Name == id) as ApplicationRole;
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            db.DeleteRole(db, userManager, role.Id);

            return RedirectToAction("Index");
        }
    }
}