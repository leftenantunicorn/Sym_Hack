using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using SymHack.Model;

namespace SymHack.Repository
{
    public class SymHackContext : IdentityDbContext<SymHackUser>
    {
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleType> ModuleTypes { get; set; }
        public DbSet<ModuleDictionary> ModuleDictionary { get; set; }
        public DbSet<ModuleHints> ModuleHints { get; set; }
        public DbSet<ModuleEmails> ModuleEmails { get; set; }
        public DbSet<ModuleHelp> ModuleHelp { get; set; }
        public DbSet<ModuleStatus> ModuleStatus { get; set; }
        public DbSet<UserModule> UserModules { get; set; }

        public SymHackContext() : base("symhackcontext", throwIfV1Schema: false) { }
    }
}
