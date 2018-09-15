namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserStates",
                c => new
                    {
                        UserStateId = c.Guid(nullable: false),
                        AccountId = c.Int(nullable: false),
                        TypeTransaction = c.Int(nullable: false),
                        TypeButton = c.Int(nullable: false),
                        TripId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.UserStateId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserStates");
        }
    }
}
