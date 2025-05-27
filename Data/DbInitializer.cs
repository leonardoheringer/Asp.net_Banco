using Banco.Models.AccountModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Banco.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Create roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@digibank.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                
                var createResult = await userManager.CreateAsync(user, "Admin@123");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    
                    // Create associated User record
                    var dbUser = new User
                    {
                        Name = "Admin",
                        Email = adminEmail,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
                    };
                    
                    context.Users.Add(dbUser);
                    await context.SaveChangesAsync();
                    
                    // Create account for admin
                    var account = new Account
                    {
                        AccountNumber = "00000001",
                        Balance = 1000000,
                        UserId = dbUser.Id
                    };
                    
                    context.Accounts.Add(account);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}