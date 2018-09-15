namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldTrip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "Phone", c => c.String());
            AddColumn("dbo.Trips", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "Description");
            DropColumn("dbo.Trips", "Phone");
        }
    }
}
