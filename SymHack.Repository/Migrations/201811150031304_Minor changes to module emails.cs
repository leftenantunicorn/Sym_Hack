namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Minorchangestomoduleemails : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Modules", name: "Module_Id", newName: "Prerequisite_Id");
            RenameIndex(table: "dbo.Modules", name: "IX_Module_Id", newName: "IX_Prerequisite_Id");
            CreateTable(
                "dbo.ModuleWinConditions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Condition = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.UserModuleEmails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        To = c.String(),
                        UserModule_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModules", t => t.UserModule_Id)
                .Index(t => t.UserModule_Id);
            
            DropColumn("dbo.ModuleEmails", "To");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModuleEmails", "To", c => c.String());
            DropForeignKey("dbo.UserModuleEmails", "UserModule_Id", "dbo.UserModules");
            DropForeignKey("dbo.ModuleWinConditions", "Module_Id", "dbo.Modules");
            DropIndex("dbo.UserModuleEmails", new[] { "UserModule_Id" });
            DropIndex("dbo.ModuleWinConditions", new[] { "Module_Id" });
            DropTable("dbo.UserModuleEmails");
            DropTable("dbo.ModuleWinConditions");
            RenameIndex(table: "dbo.Modules", name: "IX_Prerequisite_Id", newName: "IX_Module_Id");
            RenameColumn(table: "dbo.Modules", name: "Prerequisite_Id", newName: "Module_Id");
        }
    }
}
