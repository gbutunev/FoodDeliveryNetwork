using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryNetwork.Common;

namespace FoodDeliveryNetwork.Services.Data
{
    public class CourierService : ICourierService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public CourierService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<StaffViewModel>> GetCouriersByRestaurantIdAsync(string id)
        {
            bool isParsed = Guid.TryParse(id, out Guid restaurantId);
            if (isParsed && restaurantId != Guid.Empty)
            {
                return await dbContext.CourierToRestaurants
                    .Where(c => c.RestaurantId == restaurantId)
                    .Select(c => new StaffViewModel
                    {
                        UserGuid = c.CourierId,
                        UserName = c.Courier.UserName,
                        Email = c.Courier.Email,
                        FirstName = c.Courier.FirstName,
                        LastName = c.Courier.LastName,
                        PhoneNumber = c.Courier.PhoneNumber
                    })
                    .ToArrayAsync();
            }
            else
            {
                return Array.Empty<StaffViewModel>();
            }
        }

        public async Task<int> AddCourierToRestaurantAsync(string id, string newCourierEmail)
        {
            bool restaurantExists = await dbContext.Restaurants.AnyAsync(r => r.Id.ToString() == id);
            bool courierExists = await dbContext.Users.AnyAsync(u => u.Email == newCourierEmail);

            //1. Check if restaurant exists
            if (!restaurantExists)
                return -1;

            //2. Check if courier exists
            if (!courierExists)
                return -2;

            //3. Check if user is already a courier or has another role
            var courier = await userManager.FindByEmailAsync(newCourierEmail);
            var courierRoles = await userManager.GetRolesAsync(courier);
            if (courierRoles.Any())
            {
                //check if they have more than one role?
                //technically it shouldn't happen?

                var role = courierRoles.First();

                if (role != AppConstants.RoleNames.CourierRole)
                {
                    return -3;
                }
            }
            else
            {
                await userManager.AddToRoleAsync(courier, AppConstants.RoleNames.CourierRole);
            }

            try
            {
                var courierToRestaurant = new CourierToRestaurant
                {
                    CourierId = courier.Id,
                    RestaurantId = Guid.Parse(id)
                };

                await dbContext.CourierToRestaurants.AddAsync(courierToRestaurant);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> RemoveCourierFromRestaurantAsync(string id, Guid courierIdToBeDeleted)
        {
            bool restaurantExists = await dbContext.Restaurants.AnyAsync(r => r.Id.ToString() == id);
            ApplicationUser courier = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == courierIdToBeDeleted);

            //1. Check if restaurant exists
            if (!restaurantExists)
                return -1;

            //2. Check if courier exists
            if (courier is null)
                return -2;

            // --- start removing courier from restaurant ---
            try
            {
                CourierToRestaurant toDelete = await dbContext.CourierToRestaurants.FirstOrDefaultAsync(d => d.CourierId == courierIdToBeDeleted && d.RestaurantId.ToString() == id);
                if (toDelete is null)
                {
                    return -3;
                }

                dbContext.CourierToRestaurants.Remove(toDelete);

                await dbContext.SaveChangesAsync();

                //finaly check if user is a courier for any other restaurant and if not remove courier role
                bool isStillCourier = await dbContext.CourierToRestaurants.AnyAsync(c => c.CourierId == courierIdToBeDeleted);

                if (!isStillCourier)
                {
                    await userManager.RemoveFromRoleAsync(courier, AppConstants.RoleNames.CourierRole);
                }

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
