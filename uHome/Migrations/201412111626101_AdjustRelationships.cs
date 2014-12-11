namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustRelationships : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cases", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CaseAssignments", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Feedbacks", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoClips", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Cases", new[] { "ApplicationUserId" });
            DropIndex("dbo.CaseAssignments", new[] { "ApplicationUserId" });
            DropIndex("dbo.Feedbacks", new[] { "ApplicationUserId" });
            DropIndex("dbo.VideoClips", new[] { "ApplicationUserId" });
            AddColumn("dbo.Cases", "CaseAssignmentID", c => c.Int(nullable: false));
            AlterColumn("dbo.Cases", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CaseAssignments", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Feedbacks", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.VideoClips", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Cases", "ApplicationUserId");
            CreateIndex("dbo.CaseAssignments", "ApplicationUserId");
            CreateIndex("dbo.Feedbacks", "ApplicationUserId");
            CreateIndex("dbo.VideoClips", "ApplicationUserId");
            AddForeignKey("dbo.Cases", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CaseAssignments", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Feedbacks", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VideoClips", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VideoClips", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Feedbacks", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CaseAssignments", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cases", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.VideoClips", new[] { "ApplicationUserId" });
            DropIndex("dbo.Feedbacks", new[] { "ApplicationUserId" });
            DropIndex("dbo.CaseAssignments", new[] { "ApplicationUserId" });
            DropIndex("dbo.Cases", new[] { "ApplicationUserId" });
            AlterColumn("dbo.VideoClips", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Feedbacks", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.CaseAssignments", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Cases", "ApplicationUserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Cases", "CaseAssignmentID");
            CreateIndex("dbo.VideoClips", "ApplicationUserId");
            CreateIndex("dbo.Feedbacks", "ApplicationUserId");
            CreateIndex("dbo.CaseAssignments", "ApplicationUserId");
            CreateIndex("dbo.Cases", "ApplicationUserId");
            AddForeignKey("dbo.VideoClips", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Feedbacks", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CaseAssignments", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Cases", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
