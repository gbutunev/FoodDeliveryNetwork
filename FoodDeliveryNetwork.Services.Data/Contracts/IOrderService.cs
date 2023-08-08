using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Web.ViewModels.Dispatcher;
using FoodDeliveryNetwork.Web.ViewModels.Home;

namespace FoodDeliveryNetwork.Services.Data.Contracts
{
    public interface IOrderService : IBaseDataService
    {
        Task<int> ChangeOrderStatus(Guid id, OrderStatus cancelledByCustomer);
        Task<int> CreateOrder(Order order);
        Task<AllActiveOrdersViewModel> GetAllActiveOrdersByRestaurantId(Guid restaurantId, AllActiveOrdersViewModel model);
        Task<CustomerDetailedOrderViewModel> GetOrderById(string id);
        Task<CustomerAllOrdersViewModel> GetOrdersByCustomerId(CustomerAllOrdersViewModel model, string userId, int? hoursPrior = null);
        Task<bool> OrderCanBeAccessedByDispatcher(Guid orderId, string userId);
        Task<bool> OrderCanBeCancelledByDispatcher(Guid orderId);
        Task<bool> OrderCanBeCancelledByUser(Guid id, string v);
        Task<bool> OrderStatusCanBeChangedByDispatcher(Guid orderId, OrderStatus newStatus);
        Task<bool> UserHasRecentOrders(string userId, int hoursPrior);
        Task<bool> UserOwnsOrder(string userId, string orderId);
    }
}
