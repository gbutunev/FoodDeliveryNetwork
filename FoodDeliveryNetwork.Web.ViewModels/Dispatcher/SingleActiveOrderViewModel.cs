using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Dispatcher
{
    public class SingleActiveOrderViewModel
    {
        public Guid Id { get; set; }
        public string CustomerUsername { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public IEnumerable<DispatcherOrderDishViewModel> Dishes { get; set; } = new HashSet<DispatcherOrderDishViewModel>();
        public OrderStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
