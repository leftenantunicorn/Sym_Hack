using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SymHack.App_Start;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

[assembly: OwinStartupAttribute(typeof(SymHack.Startup))]
namespace SymHack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutofacConfig.RegisterComponents();
            ConfigureAuth(app);
            CreateUsersAndRoles();
        }

        // Method shell from https://code.msdn.microsoft.com/ASPNET-MVC-5-Security-And-44cbdb97
        private void CreateUsersAndRoles()
        {
            SymHackContext context = new SymHackContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<SymHackUser>(new UserStore<SymHackUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  
                var user = new SymHackUser();
                user.UserName = "larryTheUne";
                user.Email = "leftenantunicorn@gmail.com";

                string userPWD = "Mohawk1234!";

                var chkUser = userManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating teacher role    
            if (!roleManager.RoleExists("Teacher"))
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                roleManager.Create(role);

                // Create default teacher - in future move to a request admin must confirm
                var user = new SymHackUser();
                user.UserName = "";
                user.Email = "erin.bradley@mohawkcollege.ca";

                string userPWD = "Mohawk1234!";

                var chkUser = userManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Student"))
            {
                var role = new IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

            }
        }
    }
}
