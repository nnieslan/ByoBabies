namespace ByoBaby.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAspnetIdentity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PersonGroups", new[] { "Group_Id" });
            DropIndex("dbo.PersonGroups", new[] { "Person_Id" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id1" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id" });
            DropIndex("dbo.Conversations", new[] { "StartedBy_Id" });
            DropIndex("dbo.CheckIns", new[] { "Owner_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Person_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Requestor_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Audience_Id" });
            DropIndex("dbo.Notifications", new[] { "Audience_Id" });
            DropIndex("dbo.Notifications", new[] { "Originator_Id" });
            DropIndex("dbo.Children", new[] { "Parent_Id" });
            DropIndex("dbo.People", new[] { "Event_Id" });
            DropIndex("dbo.Blurbs", new[] { "WrittenBy_Id" });
            DropIndex("dbo.Blurbs", "IX_ConversationId");
            DropIndex("dbo.Blurbs", new[] { "ConversationId" });
            

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
            
            CreateIndex("dbo.Blurbs", "ConversationId", name: "IX_ConversationId");
            CreateIndex("dbo.Blurbs", "WrittenBy_Id");
            CreateIndex("dbo.People", "Event_Id");
            CreateIndex("dbo.Children", "Parent_Id");
            CreateIndex("dbo.Notifications", "Originator_Id");
            CreateIndex("dbo.Notifications", "Audience_Id");
            CreateIndex("dbo.NotificationOriginators", "Audience_Id");
            CreateIndex("dbo.NotificationOriginators", "Requestor_Id");
            CreateIndex("dbo.NotificationOriginators", "Person_Id");
            CreateIndex("dbo.CheckIns", "Owner_Id");
            CreateIndex("dbo.Conversations", "StartedBy_Id");
            CreateIndex("dbo.PersonPersons", "Person_Id");
            CreateIndex("dbo.PersonPersons", "Person_Id1");
            CreateIndex("dbo.PersonGroups", "Person_Id");
            CreateIndex("dbo.PersonGroups", "Group_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.PersonGroups", new[] { "Group_Id" });
            DropIndex("dbo.PersonGroups", new[] { "Person_Id" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id1" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Conversations", new[] { "StartedBy_Id" });
            DropIndex("dbo.CheckIns", new[] { "Owner_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Person_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Requestor_Id" });
            DropIndex("dbo.NotificationOriginators", new[] { "Audience_Id" });
            DropIndex("dbo.Notifications", new[] { "Audience_Id" });
            DropIndex("dbo.Notifications", new[] { "Originator_Id" });
            DropIndex("dbo.Children", new[] { "Parent_Id" });
            DropIndex("dbo.People", new[] { "Event_Id" });
            DropIndex("dbo.Blurbs", new[] { "WrittenBy_Id" });
            DropIndex("dbo.Blurbs", "IX_ConversationId");
            DropIndex("dbo.Blurbs", new[] { "ConversationId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
