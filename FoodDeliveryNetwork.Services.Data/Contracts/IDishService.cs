using FoodDeliveryNetwork.Web.ViewModels.Home;
using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IDishService : IBaseDataService
    {
        Task<int> AddDishToRestaurantAsync(DishFormModel model);
        Task<int> DeleteDishAsync(DishFormModel model);
        Task<int> EditDishAsync(DishFormModel model);
        Task<DishFormModel> GetDishByIdAsync(int dishId);
        Task<IEnumerable<DishViewModel>> GetOwnerDishesByRestaurantIdAsync(string id);
        Task<IEnumerable<CustomerOrderDish>> GetCustomerDishesByRestaurantIdAsync(string id);
    }
}
