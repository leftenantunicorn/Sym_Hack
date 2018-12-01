namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedprerequisitetodictionary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModuleDictionaries", "ModuleDictionary_Id", c => c.Guid());
            CreateIndex("dbo.ModuleDictionaries", "ModuleDictionary_Id");
            AddForeignKey("dbo.ModuleDictionaries", "ModuleDictionary_Id", "dbo.ModuleDictionaries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleDictionaries", "ModuleDictionary_Id", "dbo.ModuleDictionaries");
            DropIndex("dbo.ModuleDictionaries", new[] { "ModuleDictionary_Id" });
            DropColumn("dbo.ModuleDictionaries", "ModuleDictionary_Id");
        }
    }
}
