using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;

namespace SymHack.Model
{
    public class Module
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }

        public virtual ModuleType Type { get; set; }
        public virtual ICollection<ModuleDictionary> Responses { get; set; }
        public virtual ICollection<ModuleHints> Hints { get; set; }
        public virtual ICollection<ModuleEmails> Emails { get; set; }
        public virtual ICollection<ModuleHelp> Help { get; set; }
        public virtual ICollection<Module> Prerequisites { get; set; }
    }

    public class ModuleType
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ModuleDictionary
    {
        [Key]
        public Guid Id { get; set; }

        public string Request { get; set; }
        public string Response { get; set; }
    }

    public class ModuleHints
    {
        [Key]
        public Guid Id { get; set; }

        public string Topic { get; set; }
        public string Hint { get; set; }
        public int Level { get; set; }
    }

    public class ModuleEmails
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string From { get; set; }
    }

    public class ModuleHelp
    {
        [Key]
        public Guid Id { get; set; }

        public string Topic { get; set; }
        public string Description { get; set; }
    }

    public class UserModule
    {
        [Key]
        public Guid Id { get; set; }
        public string Log { get; set; }
        public virtual ModuleStatus Status { get; set; }
        public virtual Module Module { get; set; }
        public virtual SymHackUser User { get; set; }
    }

    public class ModuleStatus
    {
        [Key]
        public Guid Id { get; set; }

        public string Status { get; set; }
    }

}