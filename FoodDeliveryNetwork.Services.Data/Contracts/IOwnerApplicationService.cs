using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IOwnerApplicationService : IBaseDataService
    {
        Task AddOwnerApplicationAsync(OwnerApplication ownerApplication);
        Task<AccessToApplicationPage> CheckOwnerStatus(string userId);
    }
}
