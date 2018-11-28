namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedoptionstouser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MusicVolume", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "MusicStyle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MusicStyle");
            DropColumn("dbo.AspNetUsers", "MusicVolume");
        }
    }
}
