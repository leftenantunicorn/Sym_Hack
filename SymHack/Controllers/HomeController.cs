using System;
using System.Collections.Generic;
using System.Configuration;
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

        public async Task<ActionResult> Options()
        {
            return View(await CreateOptions());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Options(OptionsViewModel options)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if(user != null)
            {
                user.MusicStyle = options.Style;
                user.MusicVolume = options.Volume;
                await UserManager.UpdateAsync(user);
            }
            else
            {
                CookieWrapper.MusicStyle = options.Style;
                CookieWrapper.MusicVolume = options.Volume.ToString();
            }

            return View();
        }

        public async Task<Guid> GetInProgressModule(List<string> currentGame)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var gameModules =
                currentGame.Select(m => ModuleManager.GetUserModuleByModuleAndUserId(new Guid(m), user.Id));
            var currentModule = gameModules.FirstOrDefault(m => ModuleManager.GetStatusById(m.Id).Status.Equals("In Progress"));
            return currentModule?.Module?.Id ?? Guid.Empty;
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

        public async Task<ActionResult> Music()
        {
            var optionsVM = await CreateOptions();

            return PartialView("_Music", optionsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Progress()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var players = Helpers.GetPlayerModel(user, ModuleManager);

            return View("Progress", players);
        }


        [HttpPost]
        public async Task<JsonResult> GetChartData(String email)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            return Json(Helpers.GetPlayerModel(user, ModuleManager).Stats);
        }

        private async Task<OptionsViewModel>  CreateOptions()
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var optionsVM = new OptionsViewModel();

            if (user != null)
            {
                optionsVM.Volume = user.MusicVolume ?? 50;
                optionsVM.Style = user.MusicStyle ?? "mute";
            }
            else
            {
                optionsVM.Volume = Int32.TryParse(CookieWrapper.MusicVolume, out var temp) ? temp : 50;
                optionsVM.Style = CookieWrapper.MusicStyle ?? "mute";
            }

            return optionsVM;
        }
    }
}