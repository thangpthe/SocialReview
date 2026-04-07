using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SocialReview.Models;

namespace SocialReview.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            string[] roles = { "Admin", "Company", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            string adminEmail = "admin@trustify.com";
            string adminUserName = "admin";
            string adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FullName = "Administrator",
                    UserRole = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine(" Admin account created successfully.");
                }
                else
                {
                    Console.WriteLine(" Failed to create admin account:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($"   - {error.Description}");
                }
            }
            else
            {
                Console.WriteLine(" Admin account already exists.");
            }
        }
    }
}
