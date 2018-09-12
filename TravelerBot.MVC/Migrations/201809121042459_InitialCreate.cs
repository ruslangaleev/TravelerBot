namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        TripId = c.Guid(nullable: false),
                        TypeParticipant = c.Int(nullable: false),
                        From = c.Boolean(nullable: false),
                        FromString = c.String(),
                        To = c.Boolean(nullable: false),
                        ToToString = c.String(),
                        Date = c.Boolean(nullable: false),
                        Time = c.Boolean(nullable: false),
                        DateTime = c.DateTime(),
                        TimeSpan = c.Time(precision: 7),
                        AccountId = c.Int(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TripId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Trips");
        }
    }
}
