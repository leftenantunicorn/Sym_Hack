using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using SendGrid;
using SendGrid.Helpers.Mail;
using SymHack.Model;
using SymHack.Models;
using SymHack.Repository;

namespace SymHack
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            await ConfigSendGridasync(message);

        }

        private async Task ConfigSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage()
            {
                From = new EmailAddress("symhackgame@gmail.com", "Sym_Hack Admin"),
                Subject = message.Subject,
                PlainTextContent = message.Body,
                HtmlContent = message.Body
            };

            myMessage.AddTo(message.Destination);

            var credentials = new NetworkCredential(
                System.Environment.GetEnvironmentVariable("SYMHACK_MAIL"),
                System.Environment.GetEnvironmentVariable("SYMHACK_PASSWORD")
            );

            var client = new SendGridClient(System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY"));

            client.SendEmailAsync(myMessage).Wait();
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<SymHackUser>
    {
        public ApplicationUserManager(IUserStore<SymHackUser> store, IdentityFactoryOptions<ApplicationUserManager> options, IDataProtectionProvider dataProtectionProvider)
            : base(store)
        {
            // Configure validation logic for usernames
            UserValidator = new UserValidator<SymHackUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<SymHackUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<SymHackUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            EmailService = new EmailService();
            SmsService = new SmsService();
            if (dataProtectionProvider != null)
            {
                UserTokenProvider =
                    new DataProtectorTokenProvider<SymHackUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }

//        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
//        {
//            var manager = new ApplicationUserManager(new UserStore<SymHackUser>(context.Get<SymHackContext>()));
//            // Configure validation logic for usernames
//            manager.UserValidator = new UserValidator<SymHackUser>(manager)
//            {
//                AllowOnlyAlphanumericUserNames = false,
//                RequireUniqueEmail = true
//            };
//
//            // Configure validation logic for passwords
//            manager.PasswordValidator = new PasswordValidator
//            {
//                RequiredLength = 8,
//                RequireNonLetterOrDigit = true,
//                RequireDigit = false,
//                RequireLowercase = false,
//                RequireUppercase = false,
//            };
//
//            // Configure user lockout defaults
//            manager.UserLockoutEnabledByDefault = true;
//            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
//            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
//
//            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
//            // You can write your own provider and plug it in here.
//            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<SymHackUser>
//            {
//                MessageFormat = "Your security code is {0}"
//            });
//            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<SymHackUser>
//            {
//                Subject = "Security Code",
//                BodyFormat = "Your security code is {0}"
//            });
//            manager.EmailService = new EmailService();
//            manager.SmsService = new SmsService();
//            var dataProtectionProvider = options.DataProtectionProvider;
//            if (dataProtectionProvider != null)
//            {
//                manager.UserTokenProvider = 
//                    new DataProtectorTokenProvider<SymHackUser>(dataProtectionProvider.Create("ASP.NET Identity"));
//            }
//            return manager;
//        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<SymHackUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(SymHackUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

//        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
//        {
//            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
//        }
    }
}
