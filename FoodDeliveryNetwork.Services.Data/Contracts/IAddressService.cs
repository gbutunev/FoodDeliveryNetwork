using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Web.ViewModels.Home;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IAddressService : IBaseDataService
    {
        Task<bool> AddressExistsAsync(string userId, string address);
        Task<int> CreateAddress(CustomerAddress customerAddress);
        Task<int> CreateAddressAsync(string userId, string address);
        Task<int> DeleteAddressById(int id);
        Task<IEnumerable<AddressViewModel>> GetAddressesByUserId(string userId);
        Task<bool> UserOwnsAddress(string userId, int addressId);
    }
}
