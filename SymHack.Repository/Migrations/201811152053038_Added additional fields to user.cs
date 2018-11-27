namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedadditionalfieldstouser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "ExternalIdentifier", c => c.String());
            AddColumn("dbo.AspNetUsers", "RequirePasswordChange", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RequirePasswordChange");
            DropColumn("dbo.AspNetUsers", "ExternalIdentifier");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
