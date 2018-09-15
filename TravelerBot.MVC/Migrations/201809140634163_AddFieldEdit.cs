namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "EditTripId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "EditTripId");
        }
    }
}
