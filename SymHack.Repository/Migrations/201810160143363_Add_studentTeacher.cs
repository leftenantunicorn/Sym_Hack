namespace SymHack.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_studentTeacher : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Teacher_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "Teacher_Id");
            AddForeignKey("dbo.AspNetUsers", "Teacher_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Teacher_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "Teacher_Id" });
            DropColumn("dbo.AspNetUsers", "Teacher_Id");
        }
    }
}
