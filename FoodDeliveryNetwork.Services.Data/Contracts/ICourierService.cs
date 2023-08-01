using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface ICourierService : IBaseDataService
    {
        Task<int> AddCourierToRestaurantAsync(string id, string newCourierEmail);
        Task<IEnumerable<StaffViewModel>> GetCouriersByRestaurantIdAsync(string id);
        Task<int> RemoveCourierFromRestaurantAsync(string id, Guid courierIdToBeDeleted);
    }
}
