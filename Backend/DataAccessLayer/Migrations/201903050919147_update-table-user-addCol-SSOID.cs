namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetableuseraddColSSOID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SSOId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SSOId");
        }
    }
}
