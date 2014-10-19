namespace uHome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        CaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cases", t => t.CaseID, cascadeDelete: true)
                .Index(t => t.CaseID);
            
            CreateTable(
                "dbo.Cases",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.CaseAssignments",
                c => new
                    {
                        CaseID = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                        AssignmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CaseID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Cases", t => t.CaseID)
                .Index(t => t.CaseID)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Interests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.InterestMessages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        MessageBody = c.String(),
                        InterestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Interests", t => t.InterestID, cascadeDelete: true)
                .Index(t => t.InterestID);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Role_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.Role_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.Role_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.VideoClips",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        UploadedAt = c.DateTime(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.InterestApplicationUsers",
                c => new
                    {
                        Interest_ID = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Interest_ID, t.ApplicationUser_Id })
                .ForeignKey("dbo.Interests", t => t.Interest_ID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Interest_ID)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VideoClips", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CaseAssignments", "CaseID", "dbo.Cases");
            DropForeignKey("dbo.AspNetUserRoles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "Role_Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InterestMessages", "InterestID", "dbo.Interests");
            DropForeignKey("dbo.InterestApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.InterestApplicationUsers", "Interest_ID", "dbo.Interests");
            DropForeignKey("dbo.Feedbacks", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cases", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CaseAssignments", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Attachments", "CaseID", "dbo.Cases");
            DropIndex("dbo.InterestApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.InterestApplicationUsers", new[] { "Interest_ID" });
            DropIndex("dbo.VideoClips", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "Role_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.InterestMessages", new[] { "InterestID" });
            DropIndex("dbo.Feedbacks", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CaseAssignments", new[] { "ApplicationUserId" });
            DropIndex("dbo.CaseAssignments", new[] { "CaseID" });
            DropIndex("dbo.Cases", new[] { "ApplicationUserId" });
            DropIndex("dbo.Attachments", new[] { "CaseID" });
            DropTable("dbo.InterestApplicationUsers");
            DropTable("dbo.VideoClips");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.InterestMessages");
            DropTable("dbo.Interests");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CaseAssignments");
            DropTable("dbo.Cases");
            DropTable("dbo.Attachments");
        }
    }
}
