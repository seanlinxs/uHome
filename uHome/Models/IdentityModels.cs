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
        public virtual ICollection<Interest> Interests { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<CaseAssignment> CaseAssignments { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<CaseAssignment> CaseAssignments { get; set; }
        public DbSet<VideoClip> VideoClips { get; set; }
        public DbSet<InterestMessage> InterestMessages { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

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
    }
}