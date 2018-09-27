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
        public SymHackContext() : base("symhackcontext", throwIfV1Schema: false) { }

        public static SymHackContext Create()
        {
            return new SymHackContext();
        }
    }
}
