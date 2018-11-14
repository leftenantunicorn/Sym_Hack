using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SymHack.Model;

namespace SymHack.Repository
{
    public class ModuleManager
    {
        private readonly SymHackContext _context;

        public ModuleManager(SymHackContext context)
        {
            _context = context;
        }

        public Module GetModuleById(Guid id)
        {
            return _context.Modules.FirstOrDefault(m => m.Id.Equals(id));
        }

        public UserModule GetUserModuleByModuleAndUserId(Guid moduleId, string userId)
        {
            return _context.UserModules.FirstOrDefault(m => m.Module.Id.Equals(moduleId) &&
                                                            m.User.Id.Equals(userId));
        }

        public UserModule GetUserModuleById(Guid id)
        {
            return _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));
        }

        public void UpdateLogById(Guid id, string addToLog)
        {
            var userModule = _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));

            if (userModule != null)
            {
                userModule.Log += addToLog;
                _context.SaveChanges();
            }
        }
    }
}
