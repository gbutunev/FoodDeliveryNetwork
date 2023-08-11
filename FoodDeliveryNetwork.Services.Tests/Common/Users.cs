using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryNetwork.Services.Tests.Common
{
    public static class Users
    {
        public static async Task AddUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
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

            //OWNER
            var ownerUser = await userManager.FindByNameAsync("owner1");
            if (ownerUser is null)
            {
                ownerUser = new ApplicationUser
                {
                    UserName = "owner1",
                    Email = "owner1@fdn.com",
                    EmailConfirmed = true,
                    FirstName = "Owner",
                    LastName = "Ownerov",
                    PhoneNumber = "0888888887",
                    PhoneNumberConfirmed = true,
                };

                await userManager.CreateAsync(ownerUser);

                //only if no password is set
                await userManager.AddPasswordAsync(ownerUser, "owner1");
            }

            if (!await userManager.IsInRoleAsync(ownerUser, AppConstants.RoleNames.OwnerRole))
            {
                await userManager.AddToRoleAsync(ownerUser, AppConstants.RoleNames.OwnerRole);
            }

            //CUSTOMER
            var customerUser = await userManager.FindByNameAsync("customer1");
            if (customerUser is null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = "customer1",
                    Email = "customer1@fdn.com",
                    EmailConfirmed = true,
                    FirstName = "Customer",
                    LastName = "Customerov",
                    PhoneNumber = "0888888886",
                    PhoneNumberConfirmed = true,
                };

                await userManager.CreateAsync(customerUser);

                //only if no password is set
                await userManager.AddPasswordAsync(customerUser, "customer1");
            }

            //DISPATCHER - should be added as such by a restaurant owner
            var dispatcherUser = await userManager.FindByNameAsync("dispatcher1");
            if (dispatcherUser is null)
            {
                dispatcherUser = new ApplicationUser
                {
                    UserName = "dispatcher1",
                    Email = "dispatcher1@fdn.com",
                    EmailConfirmed = true,
                    FirstName = "Dispatcher",
                    LastName = "Dispatcherov",
                    PhoneNumber = "0888888885",
                    PhoneNumberConfirmed = true,
                };

                await userManager.CreateAsync(dispatcherUser);

                //only if no password is set
                await userManager.AddPasswordAsync(dispatcherUser, "dispatcher1");
            }

            //COURIER - should be added as such by a restaurant owner
            var courierUser = await userManager.FindByNameAsync("courier1");
            if (courierUser is null)
            {
                courierUser = new ApplicationUser
                {
                    UserName = "courier1",
                    Email = "courier1@fdn.com",
                    EmailConfirmed = true,
                    FirstName = "Courier",
                    LastName = "Courierov",
                    PhoneNumber = "0888888884",
                    PhoneNumberConfirmed = true,
                };

                await userManager.CreateAsync(courierUser);

                //only if no password is set
                await userManager.AddPasswordAsync(courierUser, "courier1");
            }
        }
    }
}
