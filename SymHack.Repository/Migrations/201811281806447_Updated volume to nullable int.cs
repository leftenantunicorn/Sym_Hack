namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatedvolumetonullableint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "MusicVolume", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "MusicVolume", c => c.Int(nullable: false));
        }
    }
}
