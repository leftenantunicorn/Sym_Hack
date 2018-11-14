using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;
using AutoMapper.QueryableExtensions;
using Microsoft.Ajax.Utilities;

namespace SymHack.Controllers
{
    public class GameController : Controller
    {
        private ModuleManager ModuleManager;
        private ApplicationUserManager UserManager;
        private IMapper Mapper;

        public GameController( ModuleManager monduleManager, ApplicationUserManager userManager, IMapper mapper)
        {
            ModuleManager = monduleManager;
            UserManager = userManager;
            Mapper = mapper;
        }

        // GET: Game
        public async Task<ActionResult> Index()
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var module = ModuleManager.GetModuleById(new Guid("A905EC01-225E-463F-8FE3-746F50756CDB"));

            var moduleVM = Mapper.Map<Module, ModuleViewModels>(module, opt =>
            {
                opt.Items["userId"] = user?.Id ?? "";
                opt.Items["username"] = user.UserName ?? "guest";
            });

            return View(moduleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(ModuleViewModels module)
        {
            if (ModelState.IsValid)
            {
            }

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<string> GetResponse(string key)
        {
            String response = null;

            if (ModelState.IsValid)
            {
                var module = ModuleManager.GetModuleById(new Guid("A905EC01-225E-463F-8FE3-746F50756CDB"));
                response = module.Responses.FirstOrDefault(r => new Regex(Regex.Unescape(r.Request)).IsMatch(key))?.Response;
            }

            return response != null ? Regex.Unescape(response) : $"{key} is not recognized as an internal or external command, operable program or batch file.";
        }

        [HttpGet]
        public ActionResult HandleLog(string id)
        {
            var log = ModuleManager.GetUserModuleById(new Guid(id)).Log;

            return PartialView("Log", log);
        }

        [HttpPost]
        public void HandleLog(string id, string addToLog)
        {
            if (ModelState.IsValid)
            {
                ModuleManager.UpdateLogById(new Guid(id), addToLog);
            }
        }
    }
}