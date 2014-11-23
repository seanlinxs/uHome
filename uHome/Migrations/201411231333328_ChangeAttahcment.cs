namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAttahcment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachments", "FileStream", c => c.Binary());
            AddColumn("dbo.Attachments", "UploadAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Attachments", "Path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachments", "Path", c => c.String());
            DropColumn("dbo.Attachments", "UploadAt");
            DropColumn("dbo.Attachments", "FileStream");
        }
    }
}
