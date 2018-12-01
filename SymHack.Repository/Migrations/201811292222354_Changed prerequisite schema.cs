namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changedprerequisiteschema : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ModuleDictionaries", "ModuleDictionary_Id", "dbo.ModuleDictionaries");
            DropIndex("dbo.ModuleDictionaries", new[] { "ModuleDictionary_Id" });
            AddColumn("dbo.ModuleDictionaries", "Prerequisite", c => c.String());
            AddColumn("dbo.ModuleDictionaries", "PrerequisiteReject", c => c.String());
            DropColumn("dbo.ModuleDictionaries", "ModuleDictionary_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModuleDictionaries", "ModuleDictionary_Id", c => c.Guid());
            DropColumn("dbo.ModuleDictionaries", "PrerequisiteReject");
            DropColumn("dbo.ModuleDictionaries", "Prerequisite");
            CreateIndex("dbo.ModuleDictionaries", "ModuleDictionary_Id");
            AddForeignKey("dbo.ModuleDictionaries", "ModuleDictionary_Id", "dbo.ModuleDictionaries", "Id");
        }
    }
}
