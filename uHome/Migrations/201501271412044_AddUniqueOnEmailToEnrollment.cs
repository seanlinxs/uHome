namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueOnEmailToEnrollment : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enrollments", "Email", c => c.String(maxLength: 200));
            CreateIndex("dbo.Enrollments", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Enrollments", new[] { "Email" });
            AlterColumn("dbo.Enrollments", "Email", c => c.String());
        }
    }
}
