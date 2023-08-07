using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerDetailedOrderViewModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public IEnumerable<CustomerOrderDishViewModel> Dishes { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }

        public string RestaurantHandle { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhoneNumber { get; set; }
    }
}
