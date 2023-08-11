using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Home;
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
                    OwnerId = Guid.Parse(model.OwnerId),
                    ImageGuid = model.ImageGuid
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

        public async Task<bool> RestaurantIsOwnedByUserAsync(string restaurantId, string userId)
        {
            return await dbContext
                .Restaurants
                .AnyAsync(x => x.Id.ToString() == restaurantId && x.Owner.Id.ToString() == userId);
        }

        public async Task<int> DeleteRestaurantAsync(Guid restaurantId)
        {
            var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
            if (restaurant is null) return -1;

            Guid nullRestaurantGuid = (await dbContext.Restaurants.FirstOrDefaultAsync(x => x.Handle == AppConstants.NullRestaurant)).Id;

            var orders = await dbContext.Orders.Where(x => x.RestaurantId == restaurantId).ToArrayAsync();

            try
            {
                foreach (var order in orders)
                {
                    order.RestaurantId = nullRestaurantGuid;
                }

                dbContext.Orders.UpdateRange(orders);
                await dbContext.SaveChangesAsync();

                dbContext.Restaurants.Remove(restaurant);
                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<RestaurantFormModel> GetRestaurantByIdAsync(Guid restaurantId)
        {
            return await dbContext
                .Restaurants
                .Where(x => x.Id == restaurantId)
                .Select(x => new RestaurantFormModel
                {
                    Name = x.Name,
                    Address = x.Address,
                    Handle = x.Handle,
                    Description = x.Description,
                    PhoneNumber = x.PhoneNumber,
                    OwnerId = x.OwnerId.ToString(),
                    ImageGuid = x.ImageGuid
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> EditRestaurantAsync(string id, RestaurantFormModel model)
        {
            if (id is null || id == Guid.Empty.ToString()) return -1;
            var restaurant = await dbContext
                .Restaurants
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id.ToString() == id);

            if (restaurant is null) return -1;

            var owner = await userManager.FindByIdAsync(restaurant.Owner.Id.ToString());
            if (owner is null) return -1;

            bool isOwner = await userManager.IsInRoleAsync(owner, AppConstants.RoleNames.OwnerRole);
            if (!isOwner) return -2;

            bool restaurantWithSameHandleExists = await dbContext.Restaurants.AnyAsync(x => x.Handle == model.Handle && x.Id.ToString() != id);
            if (restaurantWithSameHandleExists) return -3;
            bool restaurantWithSameNameExists = await dbContext.Restaurants.AnyAsync(x => x.Name == model.Name && x.Id.ToString() != id);
            if (restaurantWithSameNameExists) return -4;

            try
            {

                restaurant.Name = model.Name;
                restaurant.Address = model.Address;
                restaurant.Description = model.Description;
                restaurant.Handle = model.Handle;
                restaurant.PhoneNumber = model.PhoneNumber;
                restaurant.ImageGuid = model.ImageGuid;

                dbContext.Restaurants.Update(restaurant);
                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<CustomerAllRestaurantsViewModel> GetAllRestaurantsAsync(CustomerAllRestaurantsViewModel model)
        {
            model ??= new();

            var restaurants = dbContext
                .Restaurants
                .Where(x => x.Handle != AppConstants.NullRestaurant)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var wildcard = $"%{model.SearchTerm.ToLower()}%";

                restaurants = restaurants.Where(x =>
                    EF.Functions.Like(x.Name.ToLower(), wildcard) ||
                    EF.Functions.Like(x.Address.ToLower(), wildcard) ||
                    EF.Functions.Like(x.PhoneNumber.ToLower(), wildcard));
            }

            switch (model.SortBy)
            {
                case Web.ViewModels.Common.BaseQueryModelSort.Name:
                    restaurants = restaurants.OrderBy(x => x.Name);
                    break;
                case Web.ViewModels.Common.BaseQueryModelSort.Address:
                    restaurants = restaurants.OrderBy(x => x.Address);
                    break;
                default:
                    restaurants = restaurants.OrderBy(x => x.Name);
                    break;
            }

            model.TotalRestaurants = await restaurants.CountAsync();

            IEnumerable<CustomerRestaurantViewModel> restaurantsToReturn = await restaurants
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(x => new CustomerRestaurantViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Handle = x.Handle,
                    Description = x.Description,
                    PhoneNumber = x.PhoneNumber,
                    ImageGuid = x.ImageGuid
                })
                .ToArrayAsync();

            int totalRestaurants = restaurantsToReturn.Count();

            model.Restaurants = restaurantsToReturn;

            return model;
        }

        public async Task<Restaurant> GetRestaurantByHandleAsync(string id)
        {
            return await dbContext
                .Restaurants
                .Include(x => x.Dishes)
                .FirstOrDefaultAsync(x => x.Handle == id);
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(string id)
        {
            return await dbContext
                .Restaurants
                .Include(x => x.Dishes)
                .FirstOrDefaultAsync(x => x.Id.ToString() == id);
        }
    }
}
