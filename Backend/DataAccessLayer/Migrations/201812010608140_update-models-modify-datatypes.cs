namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodelsmodifydatatypes : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Users");
            DropColumn("dbo.Users", "Id");
            AddColumn("dbo.Users", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "DateOfBirth", c => c.DateTime(nullable: true));
            AlterColumn("dbo.Users", "UpdatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddPrimaryKey("dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Users", "CreatedAt", c => c.String());
            AlterColumn("dbo.Users", "UpdatedAt", c => c.String());
            AlterColumn("dbo.Users", "DateOfBirth", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.Users", "Id");
        }
    }
}
