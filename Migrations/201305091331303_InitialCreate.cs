namespace DotNetReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Feeds",
                c => new
                    {
                        FeedID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Website = c.String(),
                        Url = c.String(),
                        DisplayPublic = c.Boolean(nullable: false),
                        Lastupdate = c.Binary(),
                    })
                .PrimaryKey(t => t.FeedID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Feeds", new[] { "CategoryID" });
            DropForeignKey("dbo.Feeds", "CategoryID", "dbo.Categories");
            DropTable("dbo.Feeds");
            DropTable("dbo.Categories");
        }
    }
}
