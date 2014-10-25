namespace uHome.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using uHome.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<uHome.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "uHome.Models.ApplicationDbContext";
        }

        protected override void Seed(uHome.Models.ApplicationDbContext context)
        {
            ApplicationDbInitializer.InitializeIdentityForEF(context);
        }
    }
}
