using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class DispatcherService : IDispatcherService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public DispatcherService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<int> AddDispatcherToRestaurantAsync(string id, string newDispatcherEmail)
        {
            bool restaurantExists = await dbContext.Restaurants.AnyAsync(r => r.Id.ToString() == id);
            bool dispatcherExists = await dbContext.Users.AnyAsync(u => u.Email == newDispatcherEmail);

            //1. Check if restaurant exists
            if (!restaurantExists)
                return -1;

            //2. Check if dispatcher exists
            if (!dispatcherExists)
                return -2;

            //3. Check if dispatcher is already assigned a role
            var dispatcher = await userManager.FindByEmailAsync(newDispatcherEmail);
            var dispatcherRoles = await userManager.GetRolesAsync(dispatcher);
            if (dispatcherRoles.Any())
            {
                return -3;
            }

            //4. Check if dispatcher is already assigned to restaurant
            bool dispatcherAlreadyAssigned = await dbContext
                .DispatcherToRestaurants
                .AnyAsync(d => d.DispatcherId == dispatcher.Id && d.RestaurantId.ToString() == id);
            if (dispatcherAlreadyAssigned)
            {
                return -4;
            }

            // --- start adding dispatcher to restaurant ---
            try
            {
                await userManager.AddToRoleAsync(dispatcher, AppConstants.RoleNames.DispatcherRole);

                var dispatcherToRestaurant = new DispatcherToRestaurant
                {
                    DispatcherId = dispatcher.Id,
                    RestaurantId = Guid.Parse(id)
                };

                await dbContext.DispatcherToRestaurants.AddAsync(dispatcherToRestaurant);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public async Task<IEnumerable<StaffViewModel>> GetDispatchersByRestaurantIdAsync(string id)
        {
            bool isParsed = Guid.TryParse(id, out Guid restaurantId);
            if (isParsed && restaurantId != Guid.Empty)
            {
                return await dbContext.DispatcherToRestaurants
                .Where(d => d.RestaurantId == restaurantId)
                .Select(d => new StaffViewModel
                {
                    UserGuid = d.DispatcherId,
                    UserName = d.Dispatcher.UserName,
                    Email = d.Dispatcher.Email,
                    FirstName = d.Dispatcher.FirstName,
                    LastName = d.Dispatcher.LastName,
                    PhoneNumber = d.Dispatcher.PhoneNumber
                })
                .ToArrayAsync();
            }
            else
            {
                return Array.Empty<StaffViewModel>();
            }
        }

        public async Task<int> RemoveDispatcherFromRestaurantAsync(string id, Guid dispatcherIdToBeDeleted)
        {
            bool restaurantExists = await dbContext.Restaurants.AnyAsync(r => r.Id.ToString() == id);
            ApplicationUser dispatcher = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == dispatcherIdToBeDeleted);

            //1. Check if restaurant exists
            if (!restaurantExists)
                return -1;

            //2. Check if dispatcher exists
            if (dispatcher is null)
                return -2;

            // --- start removing dispatcher from restaurant ---
            try
            {
                await userManager.RemoveFromRoleAsync(dispatcher, AppConstants.RoleNames.DispatcherRole);

                DispatcherToRestaurant toDelete = await dbContext.DispatcherToRestaurants.FirstOrDefaultAsync(d => d.DispatcherId == dispatcherIdToBeDeleted && d.RestaurantId.ToString() == id);
                if (toDelete is null)
                {
                    return -3;
                }

                dbContext.DispatcherToRestaurants.Remove(toDelete);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<Guid> GetRestaurantIdByDispatcherId(string dispatcherId)
        {
            if (dispatcherId is null) return Guid.Empty;
            
            var r = await dbContext.DispatcherToRestaurants.FirstOrDefaultAsync(d => d.DispatcherId.ToString() == dispatcherId);

            if (r is null) return Guid.Empty;
            
            return r.RestaurantId;
        }
    }
}
