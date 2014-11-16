using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using uHome.Models;
using System.Threading.Tasks;

namespace uHome.Controllers
{
    public class RolesController : BaseController
    {
        public ActionResult Index()
        {
            List<RoleViewModel> rolesList = new List<RoleViewModel>();

            foreach (ApplicationRole role in Database.Roles)
            {
                var roleModel = new RoleViewModel(role);
                rolesList.Add(roleModel);
            }

            return View(rolesList);
        }

        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "RoleName, Description")]RoleViewModel model)
        {
            string message = "That role name has already been used";

            if (ModelState.IsValid)
            {
                var result = await RoleManager.RoleExistsAsync(model.RoleName);
                
                if (result)
                    return View(message);
                else
                {
                    ApplicationRole role = new ApplicationRole(model.RoleName, model.Description);
                    await RoleManager.CreateAsync(role);

                    return RedirectToAction("Index", "Roles");
                }
            }

            return View();
        }

        public ActionResult Edit(string id)
        {
            // It's actually the Role.Name tucked into the id param:
            ApplicationRole role = Database.Roles.First(r => r.Name == id) as ApplicationRole;
            EditRoleViewModel editRoleModel = new EditRoleViewModel(role);

            return View(editRoleModel);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "RoleName, OriginalRoleName, Description")]EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = Database.Roles.First(r => r.Name == model.OriginalRoleName) as ApplicationRole;
                role.Name = model.RoleName;
                role.Description = model.Description;
                Database.Entry(role).State = EntityState.Modified;
                Database.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationRole role = Database.Roles.First(r => r.Name == id) as ApplicationRole;
            RoleViewModel model = new RoleViewModel(role);

            if (role == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var result = await RoleManager.DeleteRoleAsync(UserManager, id);
            return RedirectToAction("Index");
        }
    }
}