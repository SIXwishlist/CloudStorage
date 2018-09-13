namespace OneDriveFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INITIAL_MIGRATION : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PerWebUserCaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        WebUserUniqueId = c.String(),
                        CacheBits = c.Binary(),
                        UserState = c.String(),
                        LastWrite = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PerWebUserCaches");
        }
    }
}
