namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInterest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Interests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Interests");
        }
    }
}
