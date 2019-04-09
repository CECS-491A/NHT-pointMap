namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetableusersaddedcolisadmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsAdministrator", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsAdministrator");
        }
    }
}
