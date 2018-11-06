using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SymHack.Model;
using SymHack.Models;

namespace SymHack.Controllers
{
    public class GameController : Controller
    {
        private ApplicationUserManager _userManager;
        private ModuleViewModels _currentModule;

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

        public GameController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            load();
        }

        public GameController()
        {
            // Eventually this will need to be able to load modules based on previous selection
            // Load will be called on the redirect
            // Add comment
            _currentModule = load().Result;
        }

        // GET: Game
        public async Task<ActionResult> Index()
        {
            return View(_currentModule);
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
                _currentModule.Responses.TryGetValue(key, out response);
            }

            return response ?? $"{key} is not recognized as an internal or external command, operable program or batch file.";
        }

        private async Task<ModuleViewModels> load()
        {
            SymHackUser user;
            try
            {
                user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            }
            catch (NullReferenceException e)
            {
                user = null;
            }

            return new ModuleViewModels
            {
                Title = "Console Module",
                ModuleType = "ConsoleModule",
                Username = user?.Email ?? "anon",
                Responses = new Dictionary<string, string>()
                {
                    {"netsh winhttp set proxy SERVER:PORT", "success"},
                    {"whois mohawkcollege.ca", "Whois v1.20 - Domain information lookup\r\nCopyright (C) 2005-2017 Mark Russinovich\r\nSysinternals - www.sysinternals.com\r\n\r\nConnecting to CA.whois-servers.net...\r\n\r\n    Name:              Mohawk College of Applied Arts and Technology\r\n\r\nAdministrative contact:\r\n    Name:              Mr IT Division Network-Group\r\n    Postal address:    135 Fennell Avenue West\r\n                       Hamilton ON L9C 1E9 Canada\r\n    Phone:             +1 (905) 575 2199\r\n    Fax:               +1 (905) 575 2197\r\n    Email:             networkgroup@mohawkcollege.ca\r\n\r\nTechnical contact:\r\n    Name:              Mr IT Division Network Group\r\n    Postal address:    135 Fennell Avenue West\r\n                       Hamilton ON L9C 1E9 Canada\r\n    Phone:             +1 (905) 575 2199\r\n    Fax:               +1 (905) 575 2197\r\n    Email:             networkgroup@mohawkcollege.ca\r\n\r\nName servers:\r\n    ns2-02.azure-dns.net\r\n    ns3-02.azure-dns.org\r\n    ns1-02.azure-dns.com\r\n    ns4-02.azure-dns.info\r\n\r\n% WHOIS look-up made at 2018-11-06 03:28:02 (GMT)\r\n%\r\n% Use of CIRA\'s WHOIS service is governed by the Terms of Use in its Legal\r\n% Notice, available at http://www.cira.ca/legal-notice/?lang=en\r\n%\r\n% (c) 2018 Canadian Internet Registration Authority, (http://www.cira.ca/)\r\n\r\nConnecting to          Mohawk College of Applied Arts and Technology...\r\nNo such host is known."},
                },
                Hints = new HashSet<string>(){
                    "Look at valid commands, and compare them to your intstructions"
                },
                Help = new HashSet<string>(){
                    "Valid commands: netsh winhttp set proxy SERVER:PORT, whois mohawkcollege.ca"
                },
                Email = new HashSet<string>(){
                    "Instructions: The first email must contain the domain name of the network to be exploited and the IP address of a proxy server.  The user must discover the name and email of the IT department head and the email of an employee to exploit.  To pass this level the user must change their internet settings to use the proxy (module 0 component) enter the email addresses for the company in an email sent to the instructor. "
                }
            };
        }
    }
}