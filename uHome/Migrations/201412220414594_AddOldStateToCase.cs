namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOldStateToCase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cases", "OldState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cases", "OldState");
        }
    }
}
