namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveAttachmentToFileSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachments", "Path", c => c.String());
            AddColumn("dbo.Attachments", "Size", c => c.Long(nullable: false));
            DropColumn("dbo.Attachments", "FileStream");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachments", "FileStream", c => c.Binary());
            DropColumn("dbo.Attachments", "Size");
            DropColumn("dbo.Attachments", "Path");
        }
    }
}
