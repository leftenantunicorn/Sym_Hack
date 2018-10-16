using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SymHack.Model
{
    // You can add profile data for the user by adding more properties to your SymHackUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class SymHackUser : IdentityUser
    {
        public SymHackUser Teacher { get; set; }
        public virtual ICollection<SymHackUser> Students { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SymHackUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}