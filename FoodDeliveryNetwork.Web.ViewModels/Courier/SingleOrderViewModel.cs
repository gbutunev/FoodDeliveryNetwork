using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Courier
{
    public class SingleOrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }

        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }

        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhoneNumber { get; set; }
    }
}
