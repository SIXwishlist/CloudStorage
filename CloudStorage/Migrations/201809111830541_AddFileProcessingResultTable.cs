namespace CloudStorage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileProcessingResultTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileProcessingResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 200),
                        CloudStorageType = c.Byte(nullable: false),
                        TimeStamp = c.DateTime(),
                        IsSuccessful = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileProcessingResults");
        }
    }
}
