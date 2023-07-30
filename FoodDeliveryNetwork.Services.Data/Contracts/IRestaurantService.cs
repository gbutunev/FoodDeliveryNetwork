﻿using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IRestaurantService : IBaseDataService
    {
        Task<int> AddRestaurant(RestaurantFormModel model);
        Task<int> DeleteRestaurantAsync(Guid restaurantId);
        Task<IEnumerable<RestaurantViewModel>> GetRestaurantsByOwnerIdAsync(string userId);
        Task<bool> RestaurantIsOwnedByUserAsync(string restaurantId, string userId);
    }
}
