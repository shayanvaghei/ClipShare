using ClipShare.Core.Entities;
using ClipShare.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Data
{
    public static class ContextInitializer
    {
        public static async Task InitializeAsync(Context context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                context.Database.Migrate();
            }

            if (!roleManager.Roles.Any())
            {
                foreach(var role in SD.Roles)
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }

            if (!userManager.Users.Any())
            {
                var admin = new AppUser
                {
                    Name = "Admin",
                    Email = "admin@example.com",
                    UserName = "admin",
                };

                await userManager.CreateAsync(admin, "Password123");
                await userManager.AddToRolesAsync(admin, [SD.AdminRole, SD.UserRole, SD.ModeratorRole]);


                var john = new AppUser
                {
                    Name = "John",
                    Email = "john@example.com",
                    UserName = "john",
                };

                await userManager.CreateAsync(john, "Password123");
                await userManager.AddToRoleAsync(john, SD.UserRole);


                var mary = new AppUser
                {
                    Name = "Mary",
                    Email = "mary@example.com",
                    UserName = "mary",
                };

                await userManager.CreateAsync(mary, "Password123");
                await userManager.AddToRoleAsync(mary, SD.ModeratorRole);
            }
        }
    }
}
