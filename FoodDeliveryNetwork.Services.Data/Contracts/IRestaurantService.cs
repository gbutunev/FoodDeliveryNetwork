using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IRestaurantService : IBaseDataService
    {
        Task<int> AddRestaurant(RestaurantFormModel model);
        Task<IEnumerable<RestaurantViewModel>> GetRestaurantsByOwnerIdAsync(string userId);
    }
}
