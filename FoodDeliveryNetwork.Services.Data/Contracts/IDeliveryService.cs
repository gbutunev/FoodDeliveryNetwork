using FoodDeliveryNetwork.Web.ViewModels.Courier;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IDeliveryService : IBaseDataService
    {
        Task<AllOrdersViewModel> GetAllArchivedOrdersByUserId(string userId, AllOrdersViewModel model);
        Task<AllOrdersViewModel> GetAllAssignedOrdersByUserId(string userId, AllOrdersViewModel model);
        Task<AllOrdersViewModel> GetAllAvailableOrdersByUserId(string userId, AllOrdersViewModel model);
    }
}