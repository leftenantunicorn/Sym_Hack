using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SymHack.Models
{
    public class ModuleViewModels
    {
        public string Title { get; set; }
        public string ModuleType { get; set; }
        public string Username { get; set; }
        public Dictionary<string, string> Responses { get; set; }
        public ICollection<string> Hints { get; set; }
        public ICollection<string> Emails { get; set; }
        public ICollection<string> Help { get; set; }
        public string Log { get; set; }
        public string UserModuleId { get; set; }
    }
}