namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveConstraintFromDownloadItem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DownloadItems", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DownloadItems", "Name", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
