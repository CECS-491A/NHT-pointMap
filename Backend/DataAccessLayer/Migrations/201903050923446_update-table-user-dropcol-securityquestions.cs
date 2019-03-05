namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetableuserdropcolsecurityquestions : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "SecurityQ1");
            DropColumn("dbo.Users", "SecurityQ1Answer");
            DropColumn("dbo.Users", "SecurityQ2");
            DropColumn("dbo.Users", "SecurityQ2Answer");
            DropColumn("dbo.Users", "SecurityQ3");
            DropColumn("dbo.Users", "SecurityQ3Answer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "SecurityQ3Answer", c => c.String());
            AddColumn("dbo.Users", "SecurityQ3", c => c.String());
            AddColumn("dbo.Users", "SecurityQ2Answer", c => c.String());
            AddColumn("dbo.Users", "SecurityQ2", c => c.String());
            AddColumn("dbo.Users", "SecurityQ1Answer", c => c.String());
            AddColumn("dbo.Users", "SecurityQ1", c => c.String());
        }
    }
}
