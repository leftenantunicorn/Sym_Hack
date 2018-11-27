using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack.Controllers
{
    public class HomeController : Controller
    {
        private ModuleManager ModuleManager;
        private ApplicationUserManager UserManager;
        private IMapper Mapper;

        public HomeController(ModuleManager moduleManager, ApplicationUserManager userManager, IMapper mapper)
        {
            ModuleManager = moduleManager;
            UserManager = userManager;
            Mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var homeVM = new HomeViewModels()
            {
                ModuleGroups = await GetModuleGroups()
            };
            return View(homeVM);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Options()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        private async Task<List<List<ModuleViewModels>>> GetModuleGroups()
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var modules = ModuleManager.GetAllModules().Select(m => Mapper.Map<ModuleViewModels>(m, opt =>
            {
                opt.Items["userId"] = user?.Id ?? "";
                opt.Items["username"] = user?.UserName ?? "guest";
            })).ToList();

            var initials = modules.Where(m => new Guid(m.PrerequisiteId) == Guid.Empty).ToList();
            var moduleGroups = new List<List<ModuleViewModels>>();

            foreach (var module in initials)
            {
                var next = Mapper.Map<ModuleViewModels>(ModuleManager.GetNextModuleById(module.Id), opt =>
                {
                    opt.Items["userId"] = user?.Id ?? "";
                    opt.Items["username"] = user?.UserName ?? "guest";
                });
                var group = new List<ModuleViewModels>() { module };
                while (next != null)
                {
                    group.Add(next);
                    next = Mapper.Map<ModuleViewModels>(ModuleManager.GetNextModuleById(next.Id), opt =>
                    {
                        opt.Items["userId"] = user?.Id ?? "";
                        opt.Items["username"] = user?.UserName ?? "guest";
                    });
                }

                moduleGroups.Add(group);
            }

            return moduleGroups;
        }
    }
}