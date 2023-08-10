using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerBlazorOrderViewModel
    {
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }

        [Required]
        [MinLength(EntityConstants.CustomerConstants.AddressMinLength)]
        [MaxLength(EntityConstants.CustomerConstants.AddressMaxLength)]
        public string Address { get; set; }

        /// <summary>
        /// DishId, Quantity
        /// </summary>
        public Dictionary<CustomerOrderDish, int> OrderItems { get; set; } = new Dictionary<CustomerOrderDish, int>();
    }
}
