using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class CourierToRestaurant
    {
        [Required]
        public Guid CourierId { get; set; }

        [ForeignKey(nameof(CourierId))]
        public ApplicationUser Courier { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public Restaurant Restaurant { get; set; }
    }
}
