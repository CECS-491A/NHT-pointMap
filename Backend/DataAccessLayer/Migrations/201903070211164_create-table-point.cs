namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createtablepoint : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Points",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Longitude = c.Single(nullable: false),
                        Latitude = c.Single(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Longitude, t.Latitude }, name: "PointLocation");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Points", "PointLocation");
            DropTable("dbo.Points");
        }
    }
}
