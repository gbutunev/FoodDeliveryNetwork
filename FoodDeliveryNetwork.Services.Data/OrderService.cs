using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Common;
using FoodDeliveryNetwork.Web.ViewModels.Dispatcher;
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

            model ??= new();
            model.BaseQueryModel ??= new();

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

        public async Task<bool> OrderCanBeCancelledByDispatcher(Guid orderId)
        {
            var order = await dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId &&
                    (x.OrderStatus == OrderStatus.Pending || x.OrderStatus == OrderStatus.Cooking || x.OrderStatus == OrderStatus.ReadyForPickup)
                );

            return order is not null;
        }

        public async Task<bool> OrderStatusCanBeChangedByDispatcher(Guid orderId, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.Cooking:
                    var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.OrderStatus == OrderStatus.Pending);
                    return order is not null;
                case OrderStatus.ReadyForPickup:
                    var order2 = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.OrderStatus == OrderStatus.Cooking);
                    return order2 is not null;
                default:
                    return false;
            }
        }

        public async Task<bool> OrderCanBeAccessedByDispatcher(Guid orderId, string userId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return await dbContext.DispatcherToRestaurants.AnyAsync(x => x.RestaurantId == order.RestaurantId && x.DispatcherId.ToString() == userId);
        }

        public async Task<bool> OrderCanBeAccessedByCourier(Guid orderId, string userId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return await dbContext.CourierToRestaurants.AnyAsync(x => x.RestaurantId == order.RestaurantId && x.CourierId.ToString() == userId);
        }

        public async Task<bool> OrderStatusCanBeChangedByCourier(Guid orderId, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.OnTheWay:
                    var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.OrderStatus == OrderStatus.ReadyForPickup);
                    return order is not null;
                case OrderStatus.Delivered:
                case OrderStatus.ReturnedToRestaurant:
                    var order2 = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId && x.OrderStatus == OrderStatus.OnTheWay);
                    return order2 is not null;
                default:
                    return false;
            }
        }

        public async Task<AllOrdersViewModel> GetAllActiveOrdersByRestaurantId(Guid restaurantId, AllOrdersViewModel model)
        {
            model ??= new();
            model.BaseQueryModel ??= new();

            var query = model.BaseQueryModel;
            var ordersQuery = dbContext.Orders.AsQueryable();

            ordersQuery = ordersQuery.Where(x => x.RestaurantId == restaurantId);
            ordersQuery = ordersQuery.Where(x => x.OrderStatus == OrderStatus.Pending ||
                                                 x.OrderStatus == OrderStatus.Cooking ||
                                                 x.OrderStatus == OrderStatus.ReadyForPickup);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var wildcard = $"%{query.SearchTerm.ToLower()}%";

                ordersQuery = ordersQuery
                    .Where(x => EF.Functions.Like(x.Customer.PhoneNumber.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.FirstName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.UserName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.LastName.ToLower(), wildcard));
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
                    ordersQuery = ordersQuery.OrderBy(x => x.CreatedOn);
                    break;
            }
            var orders = await ordersQuery
                .Include(x => x.Customer)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToArrayAsync();

            model.Orders = orders.Select(x => new SingleOrderViewModel
            {
                Id = x.Id,
                CustomerUsername = x.Customer.UserName,
                CustomerFirstName = x.Customer.FirstName,
                CustomerLastName = x.Customer.LastName,
                CustomerPhoneNumber = x.Customer.PhoneNumber,
                Address = x.Address,
                CreatedOn = x.CreatedOn,
                TotalPrice = x.TotalPrice,
                Status = x.OrderStatus,
                Dishes = x.Dishes.Select(x => new DispatcherOrderDishViewModel
                {
                    DishName = x.DishName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                }).ToArray(),
            }).ToArray();

            model.TotalOrders = model.Orders.Count();

            return model;
        }

        public async Task<AllOrdersViewModel> GetAllArchivedOrdersByRestaurantId(Guid currentRestaurant, AllOrdersViewModel model)
        {
            model ??= new();
            model.BaseQueryModel ??= new();

            var query = model.BaseQueryModel;
            var ordersQuery = dbContext.Orders.AsQueryable();

            ordersQuery = ordersQuery.Where(x => x.RestaurantId == currentRestaurant);
            ordersQuery = ordersQuery.Where(x => x.OrderStatus != OrderStatus.Pending &&
                                                 x.OrderStatus != OrderStatus.Cooking &&
                                                 x.OrderStatus != OrderStatus.ReadyForPickup);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var wildcard = $"%{query.SearchTerm.ToLower()}%";

                ordersQuery = ordersQuery
                    .Where(x => EF.Functions.Like(x.Customer.PhoneNumber.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.FirstName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.UserName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.LastName.ToLower(), wildcard));
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
                    ordersQuery = ordersQuery.OrderBy(x => x.CreatedOn);
                    break;
            }
            var orders = await ordersQuery
                .Include(x => x.Customer)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToArrayAsync();

            model.Orders = orders.Select(x => new SingleOrderViewModel
            {
                Id = x.Id,
                CustomerUsername = x.Customer.UserName,
                CustomerFirstName = x.Customer.FirstName,
                CustomerLastName = x.Customer.LastName,
                CustomerPhoneNumber = x.Customer.PhoneNumber,
                Address = x.Address,
                CreatedOn = x.CreatedOn,
                TotalPrice = x.TotalPrice,
                Status = x.OrderStatus,
                Dishes = x.Dishes.Select(x => new DispatcherOrderDishViewModel
                {
                    DishName = x.DishName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                }).ToArray(),
            }).ToArray();

            model.TotalOrders = model.Orders.Count();

            return model;
        }

        public async Task<int> AssignCourierToOrder(string userId, Guid orderId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid) || userGuid == Guid.Empty)
                return -1;

            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null)
                return -2;

            try
            {
                order.CourierId = userGuid;
                dbContext.Orders.Update(order);
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

