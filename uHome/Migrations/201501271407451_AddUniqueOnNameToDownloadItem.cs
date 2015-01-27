namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueOnNameToDownloadItem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DownloadItems", "Name", c => c.String(maxLength: 50));
            CreateIndex("dbo.DownloadItems", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.DownloadItems", new[] { "Name" });
            AlterColumn("dbo.DownloadItems", "Name", c => c.String());
        }
    }
}
