namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesessionRenameDisable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Disabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Services", "Disable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Services", "Disable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Services", "Disabled");
        }
    }
}
