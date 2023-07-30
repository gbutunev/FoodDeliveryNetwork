using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public RestaurantService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<RestaurantViewModel>> GetRestaurantsByOwnerIdAsync(string userId)
        {
            return await dbContext
                .Restaurants
                .Where(x => x.OwnerId.ToString() == userId)
                .Select(x => new RestaurantViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Handle = x.Handle,
                    Description = x.Description,
                    PhoneNumber = x.PhoneNumber,
                    TotalCouriers = x.Couriers.Count,
                    TotalDispatchers = x.Dispatchers.Count,
                    TotalDishes = x.Dishes.Count
                })
                .ToArrayAsync();
        }

        public async Task<int> AddRestaurant(RestaurantFormModel model)
        {
            if (model.OwnerId is null) return -1;

            var owner = await userManager.FindByIdAsync(model.OwnerId);
            if (owner is null) return -1;

            bool isOwner = await userManager.IsInRoleAsync(owner, AppConstants.RoleNames.OwnerRole);
            if (!isOwner) return -2;

            bool restaurantWithSameHandleExists = await dbContext.Restaurants.AnyAsync(x => x.Handle == model.Handle);
            if (restaurantWithSameHandleExists) return -3;
            bool restaurantWithSameNameExists = await dbContext.Restaurants.AnyAsync(x => x.Name == model.Name);
            if (restaurantWithSameNameExists) return -4;

            try
            {
                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    Address = model.Address,
                    Description = model.Description,
                    Handle = model.Handle,
                    PhoneNumber = model.PhoneNumber,
                    OwnerId = Guid.Parse(model.OwnerId)
                };

                await dbContext.Restaurants.AddAsync(restaurant);
                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
