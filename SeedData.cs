using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRoleAsync(roleManager);

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await EnsureTestAdminAsync(userManager);
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            var alreadyExists = await roleManager.RoleExistsAsync(Constants.AdministratorRole);
            if (alreadyExists)
            {
                return;
            }

            await roleManager.CreateAsync(new IdentityRole(Constants.AdministratorRole));
        }

        private static async Task EnsureTestAdminAsync(UserManager<IdentityUser> userManager)
        {
            const string testAdminEmail = "admin@todo.local";
            const string testAdminPassword = "NotSecure123!!!";

            var testAdmin = await userManager.Users
                .Where(u => u.UserName == testAdminEmail)
                .SingleOrDefaultAsync();

            if (testAdmin != null)
            {
                return;
            }

            testAdmin = new ApplicationUser
            {
                UserName = testAdminEmail,
                Email = testAdminEmail
            };

            await userManager.CreateAsync(testAdmin, testAdminPassword);
            await userManager.AddToRoleAsync(testAdmin, Constants.AdministratorRole);
        }
    }
}
