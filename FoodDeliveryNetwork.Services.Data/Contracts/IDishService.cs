using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IDishService : IBaseDataService
    {
        Task<int> AddDishToRestaurantAsync(DishViewModel model);
        Task<int> DeleteDishAsync(DishViewModel model);
        Task<int> EditDishAsync(DishViewModel model);
        Task<DishViewModel> GetDishByIdAsync(int dishId);
        Task<IEnumerable<DishViewModel>> GetDishesByRestaurantIdAsync(string id);
    }
}
