using FoodDeliveryNetwork.Web.ViewModels.Owner;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IDispatcherService : IBaseDataService
    {
        Task<int> AddDispatcherToRestaurantAsync(string id, string newDispatcherEmail);
        Task<IEnumerable<StaffViewModel>> GetDispatchersByRestaurantIdAsync(string id);
        Task<int> RemoveDispatcherFromRestaurantAsync(string id, Guid dispatcherIdToBeDeleted);
    }
}
