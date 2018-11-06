using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
                Students = new List<StudentViewModel>(),
                RegisterStudents = new List<StudentViewModel>()

            };

            return View(teacher);
        }

        [Authorize(Roles = "Teacher")]
        // GET: Teacher
        public async Task<ActionResult> List(TeacherViewModel teacher)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            teacher.Students = user.Students.Select(student => new StudentViewModel {Email = student.UserName}).ToList();

            return PartialView("ListStudents", teacher);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(TeacherViewModel teacher)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                var account_controller = DependencyResolver.Current.GetService<AccountController>();
                account_controller.ControllerContext =
                    new ControllerContext(this.Request.RequestContext, account_controller);

                var registration_fail = new List<StudentViewModel>();

                foreach (var student in teacher.RegisterStudents)
                {
                    // Set the default password - add functionality to force change on first login
                    var password = Membership.GeneratePassword(8, 2);

                    // Associate teacher with new student
                    SymHackUser teacher_user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    var user = new SymHackUser {UserName = student.Email, Email = student.Email, Teacher = teacher_user };

                    if (!await account_controller.RegisterUser(user, password, false, ModelState))
                        registration_fail.Add(student);
                }
                teacher.RegisterStudents = registration_fail;
            }
            
            return PartialView("RegisterStudents", teacher);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult> AddStudent()
        {
            var new_student = new StudentViewModel();
            return PartialView("AddStudent", new_student);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult> DeleteStudent(String email)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(email);
                if (user != null)
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, "Student");
                    await UserManager.DeleteAsync(user);
                }
            }

            return new EmptyResult();
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult> ViewStudent(String email)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var player = new PlayerViewModel()
                    {
                        Name = user.Email
                    };

                    return PartialView("ViewStudent", player);
                }
            }

            return PartialView("ViewStudent", null);
        }
    }
}