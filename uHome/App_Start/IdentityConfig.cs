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
using System.Web.Mvc;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Threading;
using uHome.Services;

namespace uHome.Models
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var result = MessageService.SendMail(
                ConfigurationManager.AppSettings["MailSentFrom"],
                message.Destination,
                message.Subject,
                message.Body
                );

            return result;
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

    // Configure the application user manager used in this application.
    // UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context) 
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser>(
                    context.Get<ApplicationDbContext>()));

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
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses
            // Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code",
                new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            manager.RegisterTwoFactorProvider("Email Code",
                new EmailTokenProvider<ApplicationUser>
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
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        public virtual async Task<IdentityResult> AddUserToRolesAsync(
            string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);

            // Add user to each role using UserRoleStore
            foreach (var role in roles.Where(role => !userRoles.Contains(role)))
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(
            string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);

            // Remove user to each role using UserRoleStore
            foreach (var role in roles.Where(userRoles.Contains))
            {
                await userRoleStore.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are removed
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        public ISet<ApplicationUser> GetAssigneeSet()
        {
            // Administrator, Manager and staff could be assignee
            var assigneeCandidates = new HashSet<ApplicationUser>();

            var staff = UserService.FindUsersByRoleName("Staff");
            var managers = UserService.FindUsersByRoleName("Manager");
            var admins = UserService.FindUsersByRoleName("Admin");
            var allAssignee = staff.Concat(managers).Concat(admins);

            return assigneeCandidates;
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(
                new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }

        public virtual async Task<IdentityResult> DeleteRoleAsync(
            ApplicationUserManager userManager, string roleName)
        {
            var role = await FindByNameAsync(roleName);

            foreach (var user in role.Users)
            {
                await userManager.RemoveFromRoleAsync(user.UserId, roleName);
            }

            return await DeleteAsync(role);
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(
                context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string email,
            string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await SignInAsync(user, isPersistent, false);
                return SignInStatus.Success;
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed 
                // count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);

                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }

            return SignInStatus.Failure;
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    // public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            const string email = "uhome_test@outlook.com";
            const string name = "Administrator";
            const string password = "Pass.123";

            // Create system predefined roles
            ApplicationRole[] roles = 
            {
                new ApplicationRole("Admin", "Global access"),
                new ApplicationRole("Manager", "Internal manager, can manage staff and cases"),
                new ApplicationRole("Staff", "Internal staff, can process cases"),
                new ApplicationRole("FreeAccount", "Free account"),
                new ApplicationRole("SilverAccount", "Silver account"),
                new ApplicationRole("GoldAccount", "Gold account")
            };

            foreach (var r in roles)
            {
                var role = roleManager.FindByName(r.Name);
                
                if (role == null)
                {
                    roleManager.Create(r);
                }
            }

            var user = userManager.FindByEmail(email);
            
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = email, EmailConfirmed = true };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            
            if (!rolesForUser.Contains("Admin"))
            {
                var result = userManager.AddToRole(user.Id, "Admin");
            }

            if (!rolesForUser.Contains("Manager"))
            {
                var result = userManager.AddToRole(user.Id, "Manager");
            }
        }
    }
}
