namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateuserRemoveColumnNameUserInSession : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Sessions", name: "UserInSession", newName: "UserId");
            RenameIndex(table: "dbo.Sessions", name: "IX_UserInSession", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Sessions", name: "IX_UserId", newName: "IX_UserInSession");
            RenameColumn(table: "dbo.Sessions", name: "UserId", newName: "UserInSession");
        }
    }
}
