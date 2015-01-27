namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueConstraintToEnrollment : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Enrollments");
            AlterColumn("dbo.Enrollments", "ID", c => c.Int(nullable: false));
            AlterColumn("dbo.Enrollments", "Email", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Enrollments", "Email");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Enrollments");
            AlterColumn("dbo.Enrollments", "Email", c => c.String());
            AlterColumn("dbo.Enrollments", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Enrollments", "ID");
        }
    }
}
