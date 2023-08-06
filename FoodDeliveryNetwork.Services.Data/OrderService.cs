using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryNetwork.Services.Data
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<int> CreateOrder(Order order)
        {
            //Check validity of data

            if (order.CustomerId == Guid.Empty ||
                order.RestaurantId == Guid.Empty ||
                string.IsNullOrWhiteSpace(order.Address) ||
                order.Dishes is null ||
                order.Dishes.Count() == 0)
            {
                return -1;
            }

            //Check if user exists
            var user = await userManager.FindByIdAsync(order.CustomerId.ToString());
            if (user is null) return -2;

            //Check if restaurant exists
            var restaurant = await dbContext.Restaurants.FindAsync(order.RestaurantId);
            if (restaurant is null) return -3;

            order.OrderStatus = OrderStatus.Pending;
            order.CreatedOn = DateTime.Now;

            try
            {
                await dbContext.Orders.AddAsync(order);
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
