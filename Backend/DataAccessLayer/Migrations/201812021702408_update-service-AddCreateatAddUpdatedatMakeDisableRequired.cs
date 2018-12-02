namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateserviceAddCreateatAddUpdatedatMakeDisableRequired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "UpdatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Services", "CreateAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "CreateAt");
            DropColumn("dbo.Services", "UpdatedAt");
        }
    }
}
