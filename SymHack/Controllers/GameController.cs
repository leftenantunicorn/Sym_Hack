using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Autofac.Core.Lifetime;
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

        public GameController( ModuleManager moduleManager, ApplicationUserManager userManager, IMapper mapper)
        {
            ModuleManager = moduleManager;
            UserManager = userManager;
            Mapper = mapper;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewGame(HomeViewModels homeVM)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                var gameModules =
                    homeVM.CurrentGame.Select(m => ModuleManager.GetUserModuleByModuleAndUserId(new Guid(m), user.Id))
                        .ToList();

                foreach (var module in gameModules)
                {
                    ModuleManager.UpdateStatusById(module.Id, "Not Started");
                    ModuleManager.ClearLogById(module.Id);
                    ModuleManager.DeleteUserModuleEmailsById(module.Id);
                }
            }
            else
            {
                CookieWrapper.GuestLog = "";
            }

            return RedirectToAction("Index", new { moduleId = homeVM.CurrentGame.First() });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resume(HomeViewModels homeVM)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var gameModules =
                homeVM.CurrentGame.Select(m => ModuleManager.GetUserModuleByModuleAndUserId(new Guid(m), user.Id));
            var currentModule = gameModules.FirstOrDefault(m => ModuleManager.GetStatusById(m.Id).Status.Equals("In Progress"));
            return RedirectToAction("Index", new {moduleId = currentModule?.Module?.Id ?? Guid.Empty});
        }

        // GET: Game
        public async Task<ActionResult> Index(Guid moduleId)
        {
            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var module = ModuleManager.GetModuleById(moduleId);

            if (moduleId == Guid.Empty)
            {
                return View("Error");
            }

            var moduleVM = Mapper.Map<Module, ModuleViewModels>(module, opt =>
            {
                opt.Items["userId"] = user?.Id ?? "";
                opt.Items["username"] = user?.UserName ?? "guest";
            });

            if (user == null)
            {
                moduleVM.Log = CookieWrapper.GuestLog;
                moduleVM.Outbox = ParseLogEmails(CookieWrapper.GuestLog).Select(e => Mapper.Map<UserModuleEmailsViewModels>(e)).ToList();
            }
            else
            {
                ModuleManager.UpdateStatusById(ModuleManager.GetUserModuleByModuleAndUserId(module.Id, user.Id).Id, "In Progress");
            }

            return View(moduleVM);
        }

        [HttpPost]
        public async Task<JsonResult> CheckSubmission(string userModuleId, string title)
        {
            if (!ModelState.IsValid) return Json(new { result = "Error with your submission." }); ;

            SymHackUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var module = ModuleManager.GetModuleByTitle(title);

            var log = ModuleManager?.GetUserModuleById(new Guid(userModuleId))?.Log ?? CookieWrapper.GuestLog;

            foreach (var winCondition in module.WinConditions)
            {
                if (!Regex.IsMatch(log, winCondition.Condition))
                {
                    return Json(new { result = "Requirements not yet met." });
                }
            }

            Module next;
            ModuleManager.UpdateStatusById(new Guid(userModuleId), "Completed");

            if ((next = ModuleManager.GetNextModuleById(module.Id)) != null)
            {
                return Json(new { result = "redirect", url = Url.Action("Index", "Game", new RouteValueDictionary(new { moduleId = next.Id}))});
            }

            return Json(new { result = "redirect", url=Url.Action("GameOver", "Game") });
        }

        [HttpPost]
        public async Task<string> GetResponse(string key, Guid id)
        {
            String response = null;

            if (ModelState.IsValid)
            {
                var module = ModuleManager.GetModuleById(id);
                var matches = module.Responses.Select(r =>
                {
                    var m = new Regex(Regex.Unescape(r.Request)).Match(key);
                    object[] captures = new object[m.Groups.Count];
                    m.Groups.CopyTo(captures, 0);

                    return String.Format(r.Response, captures);
                }).ToList();

                response = matches.FirstOrDefault();
            }

            return response != null ? Regex.Unescape(response) : $"{key} is not recognized as an internal or external command, operable program or batch file.";
        }

//        private bool CheckResponsePrerequisite(string key, Guid id)
//        {
//            var module = ModuleManager.GetModuleById(id);
//            var matches = module.Responses.Select(r =>
//            {
//                var m = new Regex(Regex.Unescape(r.Request)).Match(key);
//                object[] captures = new object[m.Groups.Count];
//                m.Groups.CopyTo(captures, 0);
//
//                return String.Format(r.Response, captures);
//            }).ToList();
//        }

        [HttpPost]
        public void AddToLog(string id, string addToLog)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(id))
                {
                    CookieWrapper.GuestLog += addToLog;
                    return;
                }

                ModuleManager.AddToLogById(new Guid(id), addToLog);
            }
        }

        [HttpPost]
        public PartialViewResult SendEmail(EmailViewModel emailVM)
        {
            string logEmail = $"{{\"email\":{{\"title\":\"{emailVM.NewEmail.Title}\",\"to\":\"{emailVM.NewEmail.To}\",\"body\":\"{emailVM.NewEmail.Body}\"}}}}";
            ICollection<UserModuleEmails> outgoing = new List<UserModuleEmails>();

            if (String.IsNullOrEmpty(emailVM.UserModuleId))
            {
                CookieWrapper.GuestLog += logEmail;
                outgoing = ParseLogEmails(CookieWrapper.GuestLog + logEmail);
            }
            else
            {
                ModuleManager.AddToLogById(new Guid(emailVM.UserModuleId), logEmail);

                UserModuleEmails email = new UserModuleEmails()
                {
                    Body = emailVM.NewEmail.Body,
                    Id = Guid.NewGuid(),
                    Title = emailVM.NewEmail.Title,
                    To = emailVM.NewEmail.To
                };
                
                ModuleManager.AddUserModuleEmailById(new Guid(emailVM.UserModuleId), email);
                outgoing = ModuleManager.GetOutgoingEmailsByUserModuleId(new Guid(emailVM.UserModuleId));
            }

            var incoming = ModuleManager.GetIncomingEmailsByModuleId(new Guid(emailVM.ModuleId));

            ModelState.Clear();

            return PartialView("Email", new EmailViewModel()
            {
                Inbox = incoming.Select(e => Mapper.Map<ModuleEmailsViewModels>(e)).ToList(),
                Outbox = outgoing.Select(e => Mapper.Map<UserModuleEmailsViewModels>(e)).ToList(),
                UserModuleId = emailVM.UserModuleId,
                Username = emailVM.Username,
                NewEmail = new UserModuleEmailsViewModels(),
                ModuleId = emailVM.ModuleId
            });
        }

        public async Task<ActionResult> GameOver()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GameOver(string body)
        {
            await UserManager.SendEmailAsync("30b1eb71-7d27-4e0e-ab15-0f7289d4d70d", "Feedback",
                body);

            return View();
        }

        private ICollection<UserModuleEmails> ParseLogEmails(string log)
        {
            ICollection<UserModuleEmails> emails = new List<UserModuleEmails>();
            String pattern = "{\"email\":{\"title\":\"(.+?)\",\"to\":\"(.+?)\",\"body\":\"(.+?)\"}}";
            foreach (Match m in Regex.Matches(log, pattern))
            {
                emails.Add(new UserModuleEmails()
                {
                    Title = m.Groups[1].Value,
                    Body = m.Groups[3].Value,
                    To = m.Groups[2].Value,
                });
            }

            return emails;
        } 
    }
}