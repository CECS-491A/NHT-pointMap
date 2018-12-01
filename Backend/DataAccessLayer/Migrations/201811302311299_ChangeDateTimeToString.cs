namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTimeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "DateOfBirth", c => c.String());
            AlterColumn("dbo.Users", "UpdatedAt", c => c.String());
            AlterColumn("dbo.Users", "CreatedAt", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "UpdatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
