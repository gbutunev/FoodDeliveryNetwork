﻿using FoodDeliveryNetwork.Common;
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

            try
            {
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
                    OwnerId = x.OwnerId.ToString()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> EditRestaurantAsync(string id, RestaurantFormModel model)
        {
            if (model.OwnerId is null || id is null || id == Guid.Empty.ToString()) return -1;

            var owner = await userManager.FindByIdAsync(model.OwnerId);
            if (owner is null) return -1;

            bool isOwner = await userManager.IsInRoleAsync(owner, AppConstants.RoleNames.OwnerRole);
            if (!isOwner) return -2;

            bool restaurantWithSameHandleExists = await dbContext.Restaurants.AnyAsync(x => x.Handle == model.Handle && x.Id.ToString() != id);
            if (restaurantWithSameHandleExists) return -3;
            bool restaurantWithSameNameExists = await dbContext.Restaurants.AnyAsync(x => x.Name == model.Name && x.Id.ToString() != id);
            if (restaurantWithSameNameExists) return -4;

            try
            {
                var restaurant = await dbContext.Restaurants.FindAsync(Guid.Parse(id));

                restaurant.Name = model.Name;
                restaurant.Address = model.Address;
                restaurant.Description = model.Description;
                restaurant.Handle = model.Handle;
                restaurant.PhoneNumber = model.PhoneNumber;

                dbContext.Restaurants.Update(restaurant);
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
