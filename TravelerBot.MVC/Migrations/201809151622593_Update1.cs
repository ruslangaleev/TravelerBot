namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Trips");
            AlterColumn("dbo.Trips", "TripId", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Trips", "TripId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Trips");
            AlterColumn("dbo.Trips", "TripId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Trips", "TripId");
        }
    }
}
