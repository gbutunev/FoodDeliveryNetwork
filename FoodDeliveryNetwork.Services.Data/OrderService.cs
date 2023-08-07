using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Common;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> UserHasRecentOrders(string userId, int hoursPrior)
        {
            //InvalidOperationException: The LINQ expression could not be translated
            //return await dbContext
            //    .Orders
            //    .AnyAsync(x => x.CustomerId.ToString() == userId && (
            //                        (x.CreatedOn >= DateTime.Now - TimeSpan.FromHours(hoursPrior))
            //                        ||
            //                        x.OrderStatus == OrderStatus.Pending || x.OrderStatus == OrderStatus.Cooking || x.OrderStatus == OrderStatus.OnTheWay || x.OrderStatus == OrderStatus.ReadyForPickup
            //                        )
            //              );

            var r = await dbContext
                .Orders
                .Where(x => x.CustomerId.ToString() == userId && (x.OrderStatus == OrderStatus.Pending || x.OrderStatus == OrderStatus.Cooking || x.OrderStatus == OrderStatus.OnTheWay || x.OrderStatus == OrderStatus.ReadyForPickup))
                .ToListAsync();

            return r.Any(x => x.CreatedOn >= DateTime.Now - TimeSpan.FromHours(hoursPrior));
        }

        public async Task<CustomerAllOrdersViewModel> GetOrdersByCustomerId(CustomerAllOrdersViewModel model, string userId, int? hoursPrior = null)
        {
            //if null - get all orders
            //if not null - get orders from the last x hours

            if (model is null) model = new();
            if (model.BaseQueryModel is null) model.BaseQueryModel = new();

            var query = model.BaseQueryModel;

            var ordersQuery = dbContext.Orders.AsQueryable();

            ordersQuery = ordersQuery.Where(x => x.CustomerId.ToString() == userId);

            if (hoursPrior is not null && hoursPrior > 0)
            {
                ordersQuery = ordersQuery.Where(x => x.CreatedOn >= DateTime.Now - TimeSpan.FromHours(hoursPrior.Value));
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var wildcard = $"%{query.SearchTerm.ToLower()}%";

                ordersQuery = ordersQuery
                    .Where(x => EF.Functions.Like(x.Restaurant.Name.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Restaurant.Handle.ToLower(), wildcard));
            }

            switch (query.SortBy)
            {
                case BaseQueryModelSort.Newest:
                    ordersQuery = ordersQuery.OrderByDescending(x => x.CreatedOn);
                    break;
                case BaseQueryModelSort.Oldest:
                    ordersQuery = ordersQuery.OrderBy(x => x.CreatedOn);
                    break;
                default:
                    ordersQuery = ordersQuery.OrderByDescending(x => x.CreatedOn);
                    break;
            }

            //could not be translated......
            //model.Orders = await ordersQuery
            //    .Skip((query.Page - 1) * query.PageSize)
            //    .Take(query.PageSize)
            //    .Select(x => new CustomerBasicOrderViewModel
            //    {
            //        Id = x.Id,
            //        Address = x.Address,
            //        RestaurantName = x.Restaurant.Name,
            //        RestaurantPhoneNumber = x.Restaurant.PhoneNumber,
            //        RestaurantHandle = x.Restaurant.Handle,
            //        OrderStatus = x.OrderStatus,
            //        CreatedOn = x.CreatedOn,
            //        TotalPrice = x.TotalPrice,
            //        TotalItemsCount = x.Dishes.Sum(x => x.Quantity),
            //    })
            //    .ToArrayAsync();

            var orders = await ordersQuery
                .Include(x => x.Restaurant)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToArrayAsync();

            model.Orders = orders.Select(x => new CustomerBasicOrderViewModel
            {
                Id = x.Id,
                Address = x.Address,
                RestaurantName = x.Restaurant.Name,
                RestaurantPhoneNumber = x.Restaurant.PhoneNumber,
                RestaurantHandle = x.Restaurant.Handle,
                OrderStatus = x.OrderStatus,
                CreatedOn = x.CreatedOn,
                TotalPrice = x.TotalPrice,
                TotalItemsCount = x.Dishes.Sum(x => x.Quantity),
            }).ToArray();

            model.TotalOrders = model.Orders.Count();

            return model;
        }

        public async Task<bool> OrderCanBeCancelledByUser(Guid orderId, string userId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.CustomerId.ToString() == userId && x.OrderStatus == OrderStatus.Pending);

            return order is not null;
        }

        public async Task<int> ChangeOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            var order = dbContext.Orders.FirstOrDefault(x => x.Id == orderId);

            if (order is null) return -1;

            try
            {
                order.OrderStatus = newStatus;
                dbContext.Orders.Update(order);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> UserOwnsOrder(string userId, string orderId)
        {
            return await dbContext.Orders.AnyAsync(x => x.Id.ToString() == orderId && x.CustomerId.ToString() == userId);
        }

        public async Task<CustomerDetailedOrderViewModel> GetOrderById(string id)
        {
            if (!Guid.TryParse(id, out Guid orderGuid)) return null;

            var order = await dbContext.Orders
                .Include(x => x.Restaurant)
                .FirstOrDefaultAsync(x => x.Id == orderGuid);

            if (order is null) return null;

            CustomerDetailedOrderViewModel result = new CustomerDetailedOrderViewModel()
            {
                Id = order.Id,
                Address = order.Address,
                CreatedOn = order.CreatedOn,
                OrderStatus = order.OrderStatus,
                RestaurantName = order.Restaurant.Name,
                RestaurantPhoneNumber = order.Restaurant.PhoneNumber,
                RestaurantHandle = order.Restaurant.Handle,
                TotalPrice = order.TotalPrice,
                RestaurantAddress = order.Restaurant.Address,
                Dishes = order.Dishes.Select(x => new CustomerOrderDishViewModel
                {
                    DishName = x.DishName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                }).ToArray(),
            };

            return result;
        }
    }
}

