namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateclaimguidsubjectuser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users");
            DropIndex("dbo.Claims", new[] { "SubjectUserId" });
            AlterColumn("dbo.Claims", "SubjectUserId", c => c.Guid());
            CreateIndex("dbo.Claims", "SubjectUserId");
            AddForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users");
            DropIndex("dbo.Claims", new[] { "SubjectUserId" });
            AlterColumn("dbo.Claims", "SubjectUserId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Claims", "SubjectUserId");
            AddForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
