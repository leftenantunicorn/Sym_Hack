using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using FileHelpers;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationUserManager UserManager;
        private readonly ModuleManager ModuleManager;
        private readonly IMapper Mapper;

        public TeacherController(ApplicationUserManager userManager, IMapper mapper, ModuleManager moduleManager)
        {
            UserManager = userManager;
            Mapper = mapper;
            ModuleManager = moduleManager;
        }


        [Authorize(Roles = "Teacher, Admin")]
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

        [Authorize(Roles = "Teacher, Admin")]
        // GET: Teacher
        public async Task<ActionResult> List(TeacherViewModel teacher)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            teacher.Students = user.Students.Select(student => Mapper.Map<StudentViewModel>(student)).ToList();

            return PartialView("ListStudents", teacher);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
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
                    SymHackUser teacherUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    var user = Mapper.Map<SymHackUser>(student, opt => { opt.Items["teacher_user"] = teacherUser; });

                    if (await account_controller.RegisterUser(user, password, false, ModelState))
                    {
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var email_link = Url.Action("ConfirmEmail", "Account",
                            new {userId = user.Id, code = code}, protocol: Request.Url.Scheme);

                        await UserManager.SendEmailAsync(user.Id, "Confirm your account",
                            "Please confirm your account by clicking <a href=\"" + email_link + "\">here</a>");
                    }
                    else registration_fail.Add(student);
                }

                teacher.RegisterStudents = registration_fail;
            }

            return PartialView("RegisterStudents", teacher);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher, Admin")]
        public ActionResult AddStudent()
        {
            var new_student = new StudentViewModel();
            return PartialView("AddStudent", new_student);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult> DeleteStudent(String email)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(email);
                var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    if (!currentUser.Equals(user.Teacher))
                    {
                        ViewBag.errorMessage = "The specified user is not associated with your account.";
                        return View("Error");
                    }

                    await UserManager.RemoveFromRoleAsync(user.Id, "Student");
                    ModuleManager.RemoveUserModuleByUserId(user.Id);
                    await UserManager.DeleteAsync(user);
                }
            }

            return new EmptyResult();
        }

        [HttpGet]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult> ViewStudent(String email)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(email);
                var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                if (user == null || !currentUser.Equals(user.Teacher))
                {
                    ViewBag.errorMessage = "The specified user is not associated with your account.";
                    return View("Error");
                }

                var players = Helpers.GetPlayerModel(user, ModuleManager);

                return PartialView("ViewStudent", players);
            }

            return PartialView("ViewStudent", null);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<JsonResult> GetChartData(String email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null || !currentUser.Equals(user.Teacher))
            {
                return null;
            }

            return Json(Helpers.GetPlayerModel(user, ModuleManager).Stats);
        }


        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFromFile(HttpPostedFileBase studentFile)
        {
            var teacher = new TeacherViewModel();

            if (ModelState.IsValid)
            {
                try
                {
                    StreamReader reader = new StreamReader(studentFile.InputStream);

                    var engine = new FileHelperEngine<StudentViewModel>();

                    var students = engine.ReadStream(reader).ToList();
                    teacher.RegisterStudents = students;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Could not read file");
                }
            }

            return PartialView("RegisterStudents", teacher);
        }

       
    }
}