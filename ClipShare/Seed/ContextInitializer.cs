using ClipShare.Core.Entities;
using ClipShare.DataAccess.Data;
using ClipShare.Services.IServices;
using ClipShare.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.Seed
{
    public static class ContextInitializer
    {
        public static async Task InitializeAsync(Context context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IPhotoService photoService,
            IWebHostEnvironment webHostEnvironment)
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                context.Database.Migrate();
            }

            if (!roleManager.Roles.Any())
            {
                foreach (var role in SD.Roles)
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

                var johnChannel = new Channel
                {
                    Name = "JohnChannel",
                    About = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. ",
                    AppUserId = john.Id
                };
                context.Channel.Add(johnChannel);

                var peter = new AppUser
                {
                    Name = "Peter",
                    Email = "peter@example.com",
                    UserName = "peter",
                };

                await userManager.CreateAsync(peter, "Password123");
                await userManager.AddToRoleAsync(peter, SD.UserRole);

                var peterChannel = new Channel
                {
                    Name = "PeterChannel",
                    About = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. ",
                    AppUserId = peter.Id
                };
                context.Channel.Add(peterChannel);


                var mary = new AppUser
                {
                    Name = "Mary",
                    Email = "mary@example.com",
                    UserName = "mary",
                };

                await userManager.CreateAsync(mary, "Password123");
                await userManager.AddToRoleAsync(mary, SD.ModeratorRole);



                // adding categories into our database
                var animal = new Category { Name = "Animal" };
                var food = new Category { Name = "Food" };
                var game = new Category { Name = "Game" };
                var nature = new Category { Name = "Nature" };
                var news = new Category { Name = "News" };
                var sport = new Category { Name = "Sport" };

                context.Category.Add(animal);
                context.Category.Add(food);
                context.Category.Add(game);
                context.Category.Add(nature);
                context.Category.Add(news);
                context.Category.Add(sport);

                await context.SaveChangesAsync();
            }
        }

        #region Private Helper Methods
        private static IFormFile ConvertToFile(FileInfo fileInfo)
        {
            // Open the file stream
            var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

            // Create the IFormFile instance using the file stream
            IFormFile formFile = new FormFile(
                stream,                  // File stream
                0,                       // Start position of stream
                fileInfo.Length,         // File length
                fileInfo.Name,           // Name of the form field
                fileInfo.Name            // File name
            );

            return formFile;
        }


        private static async Task<byte[]> GetContentsAsync(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            contents = memoryStream.ToArray();
            return contents;
        }
        #endregion
    }
}
