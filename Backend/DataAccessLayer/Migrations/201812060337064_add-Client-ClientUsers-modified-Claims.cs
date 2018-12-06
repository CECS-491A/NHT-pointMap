namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addClientClientUsersmodifiedClaims : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.Guid(nullable: false),
                        ClientName = c.String(nullable: false),
                        Disabled = c.Boolean(nullable: false),
                        ClientAddress = c.String(),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.ClientUsers",
                c => new
                    {
                        ClientId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.ClientId, t.UserId })
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Claims", "UserId2", c => c.Guid(nullable: false));
            AddColumn("dbo.Claims", "ClientId", c => c.Guid(nullable: false));
            //AddColumn("dbo.Claims", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            //AddColumn("dbo.Services", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            //AddColumn("dbo.Sessions", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "PasswordSalt", c => c.Binary(nullable: false));
            CreateIndex("dbo.Claims", "ClientId");
            AddForeignKey("dbo.Claims", "ClientId", "dbo.Clients", "ClientId", cascadeDelete: true);
            //DropColumn("dbo.Claims", "CreateAt");
            //DropColumn("dbo.Services", "CreateAt");
            //DropColumn("dbo.Sessions", "CreateAt");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Sessions", "CreateAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            //AddColumn("dbo.Services", "CreateAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            //AddColumn("dbo.Claims", "CreateAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropForeignKey("dbo.ClientUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.ClientUsers", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Claims", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientUsers", new[] { "UserId" });
            DropIndex("dbo.ClientUsers", new[] { "ClientId" });
            DropIndex("dbo.Claims", new[] { "ClientId" });
            AlterColumn("dbo.Users", "PasswordSalt", c => c.Binary());
            AlterColumn("dbo.Users", "PasswordHash", c => c.String());
            //DropColumn("dbo.Sessions", "CreatedAt");
            //DropColumn("dbo.Services", "CreatedAt");
            //DropColumn("dbo.Claims", "CreatedAt");
            DropColumn("dbo.Claims", "ClientId");
            DropColumn("dbo.Claims", "UserId2");
            DropTable("dbo.ClientUsers");
            DropTable("dbo.Clients");
        }
    }
}
