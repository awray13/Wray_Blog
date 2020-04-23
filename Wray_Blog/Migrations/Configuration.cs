namespace Wray_Blog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Wray_Blog.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Wray_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Wray_Blog.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }


            #region Add Users and Assign Roles

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            // You will need two of these entries to create a user to occupy the Admin Role
            // and a User to occupy the Moderater Role
            if (!context.Users.Any(u => u.Email == "aricks1986@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "aricks1986@gmail.com",
                    Email = "aricks1986@gmail.com",
                    FirstName = "Ashton",
                    LastName = "Wray",
                    DisplayName = "Ashton"
                };

                // This line creates the User in the DB
                userManager.Create(user, "Abc&123!");

                // This line attaches the Role of Admin to this specific user
                userManager.AddToRoles(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.Email == "JasonTwichell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "JasonTwichell@coderfoundry.com",
                    Email = "JasonTwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Prof"
                };

                // This line creates the User in the DB
                userManager.Create(user, "Abc&123!");

                // This line attaches the Role of Admin to this specific user
                userManager.AddToRoles(user.Id, "Moderator");
            }

            if (!context.Users.Any(u => u.Email == "ARussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "ARussell@coderfoundry.com",
                    Email = "ARussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Stache"
                };

                // This line creates the User in the DB
                userManager.Create(user, "Abc&123!");

                // This line attaches the Role of Admin to this specific user
                userManager.AddToRoles(user.Id, "Moderator");
            }
            #endregion

        }

    }
}
