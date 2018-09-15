namespace TravelerBot.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update3 : DbMigration
    {
        public override void Up()
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
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserStates", "SearchOptionId");
            DropTable("dbo.SearchOptions");
        }
    }
}
