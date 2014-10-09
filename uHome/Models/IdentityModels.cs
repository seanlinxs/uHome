using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace uHome.Models
{
    public class ApplicationRole : IdentityRole
    {
        public virtual string Description { get; set; }

        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string Name, string Description)
            : base(Name)
        {
            this.Description = Description;
        }
    }

    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole() : base()
        {
        }

        public ApplicationRole Role { get; set; }
    }

    // You can add profile data for the user by adding more properties to your
    // ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594
    // to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in
            // CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this,
                DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder is NULL");
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id)
                .ToTable("AspNetRoles");
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<ApplicationUserRole>((ApplicationUser u) => u.UserRoles);
            modelBuilder.Entity<ApplicationUserRole>()
                .HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId })
                .ToTable("AspNetUserRoles");
        }

        public bool RoleExists(ApplicationRoleManager roleManager, string name)
        {
            return roleManager.RoleExists(name);
        }

        public bool CreateRole(ApplicationRoleManager roleManager, string name, string description = "")
        {
            IdentityResult identityResult = roleManager
                .Create<ApplicationRole, string>(new ApplicationRole(name, description));
            return identityResult.Succeeded;
        }

        public bool AddUserToRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            IdentityResult identityResult = userManager.AddToRole(userId, roleName);
            return identityResult.Succeeded;
        }

        public void ClearUserRoles(ApplicationUserManager userManager, string userId)
        {
            ApplicationUser user = userManager.FindById(userId);
            List<ApplicationUserRole> currentRoles = new List<ApplicationUserRole>();

            currentRoles.AddRange(user.UserRoles);

            foreach (ApplicationUserRole role in currentRoles)
            {
                userManager.RemoveFromRole(userId, role.Role.Name);
            }
        }

        public void RemoveFromRole(ApplicationUserManager userManager, string userId, string roleName)
        {
            userManager.RemoveFromRole(userId, roleName);
        }

        public void DeleteRole(ApplicationDbContext context, ApplicationUserManager userManager, string roleId)
        {
            var roleUsers = context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == roleId));
            var role = context.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                RemoveFromRole(userManager, user.Id, role.Name);
            }
            context.Roles.Remove(role);
            context.SaveChanges();
        }
    }
}