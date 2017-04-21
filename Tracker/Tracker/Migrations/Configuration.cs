using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Tracker.Models;





namespace Tracker.Migrations
{
   

    internal sealed class Configuration : DbMigrationsConfiguration<Tracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        
            protected override void Seed(ApplicationDbContext context)
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var role = new IdentityRole();

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                role = new IdentityRole { Name = "Admin" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                role = new IdentityRole { Name = "Developer" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                role = new IdentityRole { Name = "Submitter" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                role = new IdentityRole { Name = "ProjectManager" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Tester"))
            {
                role = new IdentityRole { Name = "Tester" };
                manager.Create(role);
            }

            //context.Roles.AddOrUpdate(n => n.Name == "Tester");

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            if (!context.Users.Any(u => u.Email == "binsina9@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "binsina9@gmail.com",
                    Email = "binsina9@gmail.com",
                    FirstName = "bin",
                    LastName = "Sina",
                    DisplayName = "Binsina"
                };

                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Admin",
                        "ProjectManager",
                        "Developer",
                        "Submitter",
                        "Tester"
                    });

            }
            if (!context.Users.Any(u => u.Email == "admin@demo.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@demo.com",
                    Email = "admin@demo.com",
                    FirstName = "Administrator",
                    LastName = "Role",
                    DisplayName = "ADMIN"
                };

                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Admin"
                    });
            }
            if (!context.Users.Any(u => u.Email == "manager@demo.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "manager@demo.com",
                    Email = "manager@demo.com",
                    FirstName = "Manager",
                    LastName = "Role",
                    DisplayName = "Project Manager"
                };

                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "ProjectManager"
                    });
            }
            if (!context.Users.Any(u => u.Email == "developer@demo.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "developer@demo.com",
                    Email = "developer@demo.com",
                    FirstName = "Developer",
                    LastName = "Role",
                    DisplayName = "Developer"
                };

                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Developer"
                    });
            }
            if (!context.Users.Any(u => u.Email == "submitter@demo.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "submitter@demo.com",
                    Email = "submitter@demo.com",
                    FirstName = "Submitter",
                    LastName = "Role",
                    DisplayName = "Submitter"
                };

                userManager.Create(user, "Abc&123!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Submitter"
                    });
            }
            if (!context.Users.Any(u => u.Email == "testuser@demo.com"))
            {
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "testuser@demo.com",
                        Email = "testuser@demo.com",
                        FirstName = "Test1",
                        LastName = "User1",
                        DisplayName = "Tester"
                    }, "Abc&123!");
                }
            }
            

            if (!context.TicketPriorities.Any(u => u.Name == "Highest-Severe Issue"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Highest-Severe Issue" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "High"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "High" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Medium"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Medium" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Low"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Low" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Production Bug"))
            { context.TicketTypes.Add(new TicketType { Name = "Production Fix" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Project Task"))
            { context.TicketTypes.Add(new TicketType { Name = "Project Task" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Software Update"))
            { context.TicketTypes.Add(new TicketType { Name = "Software Update" }); }

            if (!context.TicketStatus.Any(u => u.Name == "New"))
            { context.TicketStatus.Add(new TicketStatus { Name = "New" }); }

            if (!context.TicketStatus.Any(u => u.Name == "In Process"))
            { context.TicketStatus.Add(new TicketStatus { Name = "In Process" }); }

            if (!context.TicketStatus.Any(u => u.Name == "Resolved"))
            { context.TicketStatus.Add(new TicketStatus { Name = "Resolved" }); }

            if (!context.TicketStatus.Any(u => u.Name == "Closed"))
            { context.TicketStatus.Add(new TicketStatus { Name = "Closed" }); }

            if (!context.TicketStatus.Any(u => u.Name == "Dublicate"))
            { context.TicketStatus.Add(new TicketStatus { Name = "Dublicate" }); }

            


        }
    }
}
