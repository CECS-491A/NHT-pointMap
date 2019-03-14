namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createtableuser : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SSOId = c.Guid(nullable: false),
                        Email = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PasswordHash = c.String(nullable: false),
                        PasswordSalt = c.Binary(nullable: false),
                        Disabled = c.Boolean(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");

        }
    }
}
