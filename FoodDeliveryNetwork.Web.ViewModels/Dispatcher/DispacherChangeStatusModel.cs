using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Dispatcher
{
    public class DispacherChangeStatusModel
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
