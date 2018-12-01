namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createtablesession : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sessions",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Token = c.String(nullable: false),
                    ExpiresAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreateAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    UserInSession = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserInSession, cascadeDelete: true)
                .Index(t => t.UserInSession);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "UserInSession", "dbo.Users");
            DropIndex("dbo.Sessions", new[] { "UserInSession" });
            DropTable("dbo.Sessions");

        }
    }
}
