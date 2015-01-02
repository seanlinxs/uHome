namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateEventsAndEnrollments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Number = c.String(),
                        FullName = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        EnrollAt = c.DateTime(nullable: false),
                        EventID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Events", t => t.EventID, cascadeDelete: true)
                .Index(t => t.EventID);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        OpenAt = c.DateTime(nullable: false),
                        Address = c.String(),
                        Poster = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollments", "EventID", "dbo.Events");
            DropIndex("dbo.Enrollments", new[] { "EventID" });
            DropTable("dbo.Events");
            DropTable("dbo.Enrollments");
        }
    }
}
