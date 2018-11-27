using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SymHack.Models
{
    public class ModuleViewModels
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ModuleType { get; set; }
        public string Username { get; set; }
        public Dictionary<string, string> Responses { get; set; }
        public ICollection<ModuleHintsViewModels> Hints { get; set; }
        public ICollection<ModuleEmailsViewModels> Inbox { get; set; }
        public ICollection<UserModuleEmailsViewModels> Outbox { get; set; }
        public ICollection<ModuleHelpViewModels> Help { get; set; }
        public string Log { get; set; }
        public string UserModuleId { get; set; }
        public string PrerequisiteId { get; set; }
    }

    public class ModuleHintsViewModels
    {
        public string Topic { get; set; }
        public int Level { get; set; }
        public string Hint { get; set; }
    }

    public class ModuleHelpViewModels
    {
        public string Topic { get; set; }
        public string Description { get; set; }
    }

    public class ModuleEmailsViewModels
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }

    public class UserModuleEmailsViewModels
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
    }

    public class EmailViewModel
    {
        public string Username { get; set; }
        public string UserModuleId { get; set; }
        public ICollection<ModuleEmailsViewModels> Inbox { get; set; }
        public ICollection<UserModuleEmailsViewModels> Outbox { get; set; }
        public UserModuleEmailsViewModels NewEmail { get; set; }
    }
}