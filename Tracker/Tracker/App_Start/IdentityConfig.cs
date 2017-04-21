using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Tracker.Models;
using Twilio.Types;
using Twilio;
using Twilio.TwiML;



using Twilio.Clients;
using System.Web.Configuration;
using System.Net.Mail;
using System.Net;

namespace Tracker
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await SendMailAsync(message);
        }
        public async Task<bool> SendMailAsync(IdentityMessage message)
        {

            //Private.Config setting
            var SMTP_USERNAME = WebConfigurationManager.AppSettings["SMTP_USERNAME"];
            var SMTP_PASSWORD = WebConfigurationManager.AppSettings["SMTP_PASSWORD"];
            var HOST = WebConfigurationManager.AppSettings["HOST"];
            int PORT = Convert.ToInt32(WebConfigurationManager.AppSettings["PORT"]);
            //Email coming From: it will say=> BugTracker
            var FROM = new MailAddress(WebConfigurationManager.AppSettings["emailfrom"], "BugTracker");

            var email = new MailMessage(FROM, new MailAddress
                (message.Destination))

            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true,
            };
            //STMP Object set-up
            using (var smtp = new SmtpClient()
            {
                Host = HOST,
                Port = PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD)
            })
                try
                {
                    await smtp.SendMailAsync(email);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
        }

    };




public class SmsService : IIdentityMessageService
{
    public Task SendAsync(IdentityMessage message)
    {

        //string AccountSid = "YourTwilioAccountSID";
        //string AuthToken = "YourTwilioAuthToken";
        //string twilioPhoneNumber = "YourTwilioPhoneNumber";

        //var twilio = new TwilioRestClient(AccountSid, AuthToken);
        ////twilio.SendSmsMessage(twilioPhoneNumber, message.Destination, message.Body);

        //// Twilio does not return an async Task, so we need this:
        return Task.FromResult(0);
    }
}

// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
public class ApplicationUserManager : UserManager<ApplicationUser>
{
    public ApplicationUserManager(IUserStore<ApplicationUser> store)
        : base(store)
    {
    }

    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    {
        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
        // Configure validation logic for usernames
        manager.UserValidator = new UserValidator<ApplicationUser>(manager)
        {
            AllowOnlyAlphanumericUserNames = false,
            RequireUniqueEmail = true
        };

        // Configure validation logic for passwords
        manager.PasswordValidator = new PasswordValidator
        {
            RequiredLength = 6,
            RequireNonLetterOrDigit = true,
            RequireDigit = true,
            RequireLowercase = true,
            RequireUppercase = true,
        };

        // Configure user lockout defaults
        manager.UserLockoutEnabledByDefault = true;
        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
        manager.MaxFailedAccessAttemptsBeforeLockout = 5;

        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
        // You can write your own provider and plug it in here.
        manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
        {
            MessageFormat = "Your security code is {0}"
        });
        manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
        {
            Subject = "Security Code",
            BodyFormat = "Your security code is {0}"
        });
        manager.EmailService = new EmailService();
        manager.SmsService = new SmsService();
        var dataProtectionProvider = options.DataProtectionProvider;
        if (dataProtectionProvider != null)
        {
            manager.UserTokenProvider =
                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
        }
        return manager;
    }
}

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
