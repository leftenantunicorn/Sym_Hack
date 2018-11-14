namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Module_Id = c.Guid(),
                        Type_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .ForeignKey("dbo.ModuleTypes", t => t.Type_Id)
                .Index(t => t.Module_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.ModuleTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserModules",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Log = c.String(),
                        Module_Id = c.Guid(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Module_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserModules", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserModules", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.Modules", "Type_Id", "dbo.ModuleTypes");
            DropForeignKey("dbo.Modules", "Module_Id", "dbo.Modules");
            DropIndex("dbo.UserModules", new[] { "User_Id" });
            DropIndex("dbo.UserModules", new[] { "Module_Id" });
            DropIndex("dbo.Modules", new[] { "Type_Id" });
            DropIndex("dbo.Modules", new[] { "Module_Id" });
            DropTable("dbo.UserModules");
            DropTable("dbo.ModuleTypes");
            DropTable("dbo.Modules");
        }
    }
}
