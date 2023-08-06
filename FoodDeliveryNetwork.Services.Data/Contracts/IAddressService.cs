using FoodDeliveryNetwork.Web.ViewModels.Home;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IAddressService : IBaseDataService
    {
        Task<bool> AddressExistsAsync(string userId, string address);
        Task<int> CreateAddressAsync(string userId, string address);
        Task<IEnumerable<AddressViewModel>> GetAddressesByUserId(string userId);
    }
}
