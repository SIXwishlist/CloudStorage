namespace OneDriveFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableStructure2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PerWebUserCaches", "WebUserUniqueId", c => c.String());
            DropColumn("dbo.PerWebUserCaches", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PerWebUserCaches", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.PerWebUserCaches", "WebUserUniqueId");
        }
    }
}
