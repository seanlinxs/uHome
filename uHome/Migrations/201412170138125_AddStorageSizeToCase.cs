namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStorageSizeToCase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "StorageSize", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "StorageSize");
        }
    }
}
