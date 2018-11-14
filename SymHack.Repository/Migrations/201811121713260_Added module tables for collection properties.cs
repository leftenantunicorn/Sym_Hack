namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedmoduletablesforcollectionproperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleDictionaries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Request = c.String(),
                        Response = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleEmails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Body = c.String(),
                        To = c.String(),
                        From = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleHelps",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Topic = c.String(),
                        Description = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleHints",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Topic = c.String(),
                        Hint = c.String(),
                        Level = c.Int(nullable: false),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserModules", "Status_Id", c => c.Guid());
            CreateIndex("dbo.UserModules", "Status_Id");
            AddForeignKey("dbo.UserModules", "Status_Id", "dbo.ModuleStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserModules", "Status_Id", "dbo.ModuleStatus");
            DropForeignKey("dbo.ModuleDictionaries", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.ModuleHints", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.ModuleHelps", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.ModuleEmails", "Module_Id", "dbo.Modules");
            DropIndex("dbo.UserModules", new[] { "Status_Id" });
            DropIndex("dbo.ModuleHints", new[] { "Module_Id" });
            DropIndex("dbo.ModuleHelps", new[] { "Module_Id" });
            DropIndex("dbo.ModuleEmails", new[] { "Module_Id" });
            DropIndex("dbo.ModuleDictionaries", new[] { "Module_Id" });
            DropColumn("dbo.UserModules", "Status_Id");
            DropTable("dbo.ModuleStatus");
            DropTable("dbo.ModuleHints");
            DropTable("dbo.ModuleHelps");
            DropTable("dbo.ModuleEmails");
            DropTable("dbo.ModuleDictionaries");
        }
    }
}
