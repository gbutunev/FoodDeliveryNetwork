using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.AspNetCore.Identity;

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

                if (!await roleManager.RoleExistsAsync(AppConstants.AdministratorRoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(AppConstants.AdministratorRoleName));
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

                if (!await userManager.IsInRoleAsync(adminUser, AppConstants.AdministratorRoleName))
                {
                    await userManager.AddToRoleAsync(adminUser, AppConstants.AdministratorRoleName);
                }
            }
        }
    }
}
