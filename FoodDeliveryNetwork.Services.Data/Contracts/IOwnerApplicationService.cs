using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Web.ViewModels.Admin;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IOwnerApplicationService : IBaseDataService
    {
        Task AddOwnerApplicationAsync(OwnerApplication ownerApplication);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <returns>-2 - general error | -1 - null parameters | 0 - exception | 1 - success</returns>
        Task<int> ChangeApplicationStatusAsync(int id, OwnerApplicationStatus? newStatus);
        Task<AccessToApplicationPage> CheckOwnerStatus(string userId);
        Task<AllApplicationsViewModel> GetAllApplicationsAsync(AllApplicationsViewModel model, bool archived);
        Task<ApplicationChangeStatusViewModel> GetApplicationByIdAsync(int id);
    }
}
