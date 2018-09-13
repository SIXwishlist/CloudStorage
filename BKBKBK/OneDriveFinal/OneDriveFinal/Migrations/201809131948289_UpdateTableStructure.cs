namespace OneDriveFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableStructure : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PerWebUserCaches", "WebUserUniqueId");
            DropColumn("dbo.PerWebUserCaches", "UserState");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PerWebUserCaches", "UserState", c => c.String());
            AddColumn("dbo.PerWebUserCaches", "WebUserUniqueId", c => c.String());
        }
    }
}
