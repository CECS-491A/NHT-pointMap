namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateclaimAddCreatedAtAddUpdateAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "UpdatedAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Claims", "CreateAt", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "CreateAt");
            DropColumn("dbo.Claims", "UpdatedAt");
        }
    }
}
