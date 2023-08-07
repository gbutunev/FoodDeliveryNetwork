using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerBasicOrderViewModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantPhoneNumber { get; set; }
        public string RestaurantHandle { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItemsCount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
