namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExtraColumnsToDownloadItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DownloadItems", "FileName", c => c.String());
            AddColumn("dbo.DownloadItems", "Size", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DownloadItems", "Size");
            DropColumn("dbo.DownloadItems", "FileName");
        }
    }
}
