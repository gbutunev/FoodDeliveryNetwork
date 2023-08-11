using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using FoodDeliveryNetwork.Web.ViewModels.Courier;
using FoodDeliveryNetwork.Web.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public DeliveryService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<AllOrdersViewModel> GetAllAvailableOrdersByUserId(string userId, AllOrdersViewModel model)
        {
            model ??= new();

            HashSet<Guid> restaurants = dbContext.CourierToRestaurants.Where(x => x.CourierId.ToString() == userId).Select(x => x.RestaurantId).ToHashSet();

            var r = dbContext.Orders
                            .Include(x => x.Customer)
                            .Include(x => x.Restaurant)
                            .Where(x => restaurants.Contains(x.RestaurantId))
                            .Where(x => x.CourierId == null)
                            .Where(x => x.OrderStatus == OrderStatus.ReadyForPickup)
                            .ApplySearchQuery(model);

            model.TotalOrders = await r.CountAsync();

            model.Orders = await r
                    .ApplyPaginationQuery(model)
                    .Select(x => new SingleOrderViewModel()
                    {
                        Id = x.Id,
                        CreatedOn = x.CreatedOn,
                        OrderStatus = x.OrderStatus,
                        TotalPrice = x.TotalPrice,

                        CustomerFirstName = x.Customer.FirstName,
                        CustomerLastName = x.Customer.LastName,
                        CustomerPhoneNumber = x.Customer.PhoneNumber,
                        CustomerAddress = x.Address,

                        RestaurantName = x.Restaurant.Name,
                        RestaurantAddress = x.Restaurant.Address,
                        RestaurantPhoneNumber = x.Restaurant.PhoneNumber
                    })
                    .ToArrayAsync();

            return model;
        }

        public async Task<AllOrdersViewModel> GetAllAssignedOrdersByUserId(string userId, AllOrdersViewModel model)
        {
            model ??= new();

            HashSet<Guid> restaurants = dbContext.CourierToRestaurants.Where(x => x.CourierId.ToString() == userId).Select(x => x.RestaurantId).ToHashSet();

            var r = dbContext.Orders
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .Where(x => restaurants.Contains(x.RestaurantId))
                .Where(x => x.CourierId.ToString() == userId)
                .Where(x => x.OrderStatus == OrderStatus.OnTheWay)
                .ApplySearchQuery(model);

            model.TotalOrders = await r.CountAsync();

            model.Orders = await r
                .ApplyPaginationQuery(model)
                .Select(x => new SingleOrderViewModel()
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    OrderStatus = x.OrderStatus,
                    TotalPrice = x.TotalPrice,

                    CustomerFirstName = x.Customer.FirstName,
                    CustomerLastName = x.Customer.LastName,
                    CustomerPhoneNumber = x.Customer.PhoneNumber,
                    CustomerAddress = x.Address,

                    RestaurantName = x.Restaurant.Name,
                    RestaurantAddress = x.Restaurant.Address,
                    RestaurantPhoneNumber = x.Restaurant.PhoneNumber
                })
                .ToArrayAsync();

            return model;
        }

        public async Task<AllOrdersViewModel> GetAllArchivedOrdersByUserId(string userId, AllOrdersViewModel model)
        {
            model ??= new();

            HashSet<Guid> restaurants = dbContext.CourierToRestaurants.Where(x => x.CourierId.ToString() == userId).Select(x => x.RestaurantId).ToHashSet();

            var r = dbContext.Orders
                .Include(x => x.Customer)
                .Include(x => x.Restaurant)
                .Where(x => restaurants.Contains(x.RestaurantId))
                .Where(x => x.CourierId.ToString() == userId)
                .Where(x => x.OrderStatus != OrderStatus.OnTheWay)
                .ApplySearchQuery(model);

            model.TotalOrders = await r.CountAsync();

            model.Orders = await r
                .ApplyPaginationQuery(model)
                .Select(x => new SingleOrderViewModel()
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    OrderStatus = x.OrderStatus,
                    TotalPrice = x.TotalPrice,

                    CustomerFirstName = x.Customer.FirstName,
                    CustomerLastName = x.Customer.LastName,
                    CustomerPhoneNumber = x.Customer.PhoneNumber,
                    CustomerAddress = x.Address,

                    RestaurantName = x.Restaurant.Name,
                    RestaurantAddress = x.Restaurant.Address,
                    RestaurantPhoneNumber = x.Restaurant.PhoneNumber
                })
                .ToArrayAsync();


            return model;
        }
    }

    public static class TestExtensions
    {
        public static IQueryable<T> ApplySearchQuery<T>(this IQueryable<T> query, AllOrdersViewModel model) where T : Order
        {
            model ??= new();

            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var wildcard = $"%{model.SearchTerm.ToLower()}%";

                query = query
                    .Where(x => EF.Functions.Like(x.Customer.FirstName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.LastName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Customer.PhoneNumber.ToLower(), wildcard) ||
                                EF.Functions.Like(x.Address.ToLower(), wildcard));
            }

            switch (model.SortBy)
            {
                case BaseQueryModelSort.Newest:
                    query = query.OrderByDescending(x => x.CreatedOn);
                    break;
                case BaseQueryModelSort.Oldest:
                    query = query.OrderBy(x => x.CreatedOn);
                    break;
                default:
                    query = query.OrderBy(x => x.CreatedOn);
                    break;
            }

            return query;
        }

        public static IQueryable<T> ApplyPaginationQuery<T>(this IQueryable<T> query, AllOrdersViewModel model) where T : Order
        {
            model ??= new();

            query = query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize);

            return query;
        }
    }
}
