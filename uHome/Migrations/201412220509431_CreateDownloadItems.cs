namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDownloadItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DownloadItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DownloadItems");
        }
    }
}
