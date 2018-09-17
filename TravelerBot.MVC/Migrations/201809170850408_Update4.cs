namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "Whence", c => c.String());
            AddColumn("dbo.Trips", "Where", c => c.String());
            AddColumn("dbo.Trips", "Comments", c => c.String());
            AddColumn("dbo.Trips", "UserStateId", c => c.Guid(nullable: false));
            AddColumn("dbo.UserStates", "Filter", c => c.String());
            CreateIndex("dbo.Trips", "UserStateId");
            AddForeignKey("dbo.Trips", "UserStateId", "dbo.UserStates", "UserStateId", cascadeDelete: true);
            DropColumn("dbo.Trips", "From");
            DropColumn("dbo.Trips", "FromString");
            DropColumn("dbo.Trips", "To");
            DropColumn("dbo.Trips", "ToToString");
            DropColumn("dbo.Trips", "Date");
            DropColumn("dbo.Trips", "Time");
            DropColumn("dbo.Trips", "TimeSpan");
            DropColumn("dbo.Trips", "AccountId");
            DropColumn("dbo.Trips", "TypeTransaction");
            DropColumn("dbo.Trips", "EditTripId");
            DropColumn("dbo.Trips", "Description");
            DropColumn("dbo.UserStates", "SearchOptionId");
            DropTable("dbo.SearchOptions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SearchOptions",
                c => new
                    {
                        SearchOptionsId = c.Guid(nullable: false),
                        Filter = c.String(),
                    })
                .PrimaryKey(t => t.SearchOptionsId);
            
            AddColumn("dbo.UserStates", "SearchOptionId", c => c.Guid(nullable: false));
            AddColumn("dbo.Trips", "Description", c => c.String());
            AddColumn("dbo.Trips", "EditTripId", c => c.Guid(nullable: false));
            AddColumn("dbo.Trips", "TypeTransaction", c => c.Int(nullable: false));
            AddColumn("dbo.Trips", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Trips", "TimeSpan", c => c.Time(precision: 7));
            AddColumn("dbo.Trips", "Time", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trips", "Date", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trips", "ToToString", c => c.String());
            AddColumn("dbo.Trips", "To", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trips", "FromString", c => c.String());
            AddColumn("dbo.Trips", "From", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Trips", "UserStateId", "dbo.UserStates");
            DropIndex("dbo.Trips", new[] { "UserStateId" });
            DropColumn("dbo.UserStates", "Filter");
            DropColumn("dbo.Trips", "UserStateId");
            DropColumn("dbo.Trips", "Comments");
            DropColumn("dbo.Trips", "Where");
            DropColumn("dbo.Trips", "Whence");
        }
    }
}
