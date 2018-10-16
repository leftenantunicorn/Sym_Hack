using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;
using SymHack.Repository.Migrations;
using WebGrease.Css.Extensions;

namespace SymHack.Controllers
{
    public class TeacherController : Controller
    {
        private ApplicationUserManager _userManager;

        public TeacherController()
        {
        }

        public TeacherController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Teacher")]
        // GET: Teacher
        public async Task<ActionResult> Index()
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            TeacherViewModel teacher = new TeacherViewModel
            {
                Students = user.Students.Select(student => new StudentViewModel {Name = student.UserName}).ToList()
            };
            return View(teacher);
        }
    }
}