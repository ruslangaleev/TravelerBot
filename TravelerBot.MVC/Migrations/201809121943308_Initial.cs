namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trips", "TypeTransaction", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trips", "TypeTransaction");
        }
    }
}
