using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace FoodDeliveryNetwork.Web.Extensions
{
    public static class SeedDefaultAccounts
    {
        public static async Task SeedDefaultAccountsAsync(this WebApplication webApplication)
        {
            using (IServiceScope scope = webApplication.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //iterate through all roles and create them if they don't exist
                Type roleNamesType = typeof(AppConstants.RoleNames);
                FieldInfo[] fields = roleNamesType.GetFields(BindingFlags.Public | BindingFlags.Static);
                var roleFields = fields.Where(f => f.FieldType == typeof(string));

                foreach (var roleField in roleFields)
                {
                    var roleName = roleField.GetValue(null) as string;

                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    }
                }

                //check if user with username admin1 exists and if they have admin role
                var adminUser = await userManager.FindByNameAsync("admin1");
                if (adminUser is null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin1",
                        Email = "admin1@fdn.com",
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "Adminov",
                        PhoneNumber = "0888888888",
                        PhoneNumberConfirmed = true,                        
                    };

                    await userManager.CreateAsync(adminUser);

                    //only if no password is set
                    await userManager.AddPasswordAsync(adminUser, "admin1");
                }

                if (!await userManager.IsInRoleAsync(adminUser, AppConstants.RoleNames.AdministratorRole))
                {
                    await userManager.AddToRoleAsync(adminUser, AppConstants.RoleNames.AdministratorRole);
                }
            }
        }
    }
}
