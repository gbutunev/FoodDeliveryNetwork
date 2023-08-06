using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IOrderService : IBaseDataService
    {
        Task<int> CreateOrder(Order order);
    }
}
