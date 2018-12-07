namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tester : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Claims", "ClientId", "dbo.Clients");
            DropIndex("dbo.Claims", new[] { "ClientId" });
            AlterColumn("dbo.Claims", "ClientId", c => c.Guid());
            CreateIndex("dbo.Claims", "ClientId");
            AddForeignKey("dbo.Claims", "ClientId", "dbo.Clients", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Claims", "ClientId", "dbo.Clients");
            DropIndex("dbo.Claims", new[] { "ClientId" });
            AlterColumn("dbo.Claims", "ClientId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Claims", "ClientId");
            AddForeignKey("dbo.Claims", "ClientId", "dbo.Clients", "Id", cascadeDelete: true);
        }
    }
}
