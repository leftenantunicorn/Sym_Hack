using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public Module GetModuleByTitle(string title)
        {
            return _context.Modules.FirstOrDefault(m => m.Title.Equals(title));
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

        public void AddUserModuleByModuleAndUser(Module module, SymHackUser user)
        {
            _context.UserModules.Add(new UserModule()
            {
                Id = Guid.NewGuid(),
                Module = module,
                User = user,
                Status = _context.ModuleStatus.FirstOrDefault(ms => ms.Status.Equals("Not Started")),
                Log = ""
            });
        }

        public void RemoveUserModuleByUserId(string userId)
        {
            var modules = _context.Modules;
            ICollection<UserModule> userModules = new List<UserModule>();
            foreach (var module in modules)
            {
                var userModule =
                    _context.UserModules.FirstOrDefault(um => um.Module.Id.Equals(module.Id) && um.User.Id.Equals(userId));
                if (userModule != null)
                {
                    userModules.Add(userModule);
                }
            }

            _context.UserModules.RemoveRange(userModules);
        }

        public void AddToLogById(Guid id, string addToLog)
        {
            var userModule = _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));

            if (userModule != null)
            {
                userModule.Log += addToLog;
                _context.SaveChanges();
            }
        }

        public void ClearLogById(Guid id)
        {
            var userModule = _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));

            if (userModule != null)
            {
                userModule.Log = "";
                _context.SaveChanges();
            }
        }

        public void UpdateStatusById(Guid id, string status)
        {
            var userModule = _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));
            var userStatus = _context.ModuleStatus.FirstOrDefault(s => s.Status.Equals(status));

            if (userModule != null )
            {
                userModule.Status = userStatus;
                _context.SaveChanges();
            }
        }

        public ModuleStatus GetStatusById(Guid id)
        {
            return _context.UserModules.FirstOrDefault(m => m.Id.Equals(id))?.Status;
        }

        public void AddUserModuleEmailById(Guid id, UserModuleEmails email)
        {
            var userModule = _context.UserModules.FirstOrDefault(m => m.Id.Equals(id));
            
            if (userModule != null)
            {
                userModule.ModuleEmails.Add(email);
                _context.SaveChanges();
            }
        }

        public ICollection<UserModuleEmails> GetOutgoingEmailsByUserModuleId(Guid id)
        {
            return _context.UserModules.FirstOrDefault(m => m.Id.Equals(id))?.ModuleEmails;
        }

        public ICollection<ModuleEmails> GetIncomingEmailsByUserModuleId(Guid id)
        {
            var moduleId = _context.UserModules.FirstOrDefault(um => um.Id.Equals(id))?.Module.Id;
            var module = _context.Modules.FirstOrDefault(m => m.Id == moduleId);
            return module?.Emails;
        }

        public ICollection<ModuleEmails> GetIncomingEmailsByModuleId(Guid id)
        {
            var module = _context.Modules.FirstOrDefault(m => m.Id == id);
            return module?.Emails;
        }

        public void DeleteUserModuleEmailsById(Guid id)
        {
            var userModule = _context.UserModuleEmails.Where(m => m.UserModule.Id.Equals(id));
            _context.UserModuleEmails.RemoveRange(userModule);
            _context.SaveChanges();
        }

        public Module GetNextModuleById(Guid id)
        {
            return _context.Modules.FirstOrDefault(m => m.Prerequisite.Id.Equals(id));
        }

        public ICollection<Module> GetAllModules()
        {
            return _context.Modules.ToList();
        }
    }
}
