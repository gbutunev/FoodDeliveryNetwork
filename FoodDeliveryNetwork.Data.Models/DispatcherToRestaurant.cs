using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Data.Models
{    
    public class DispatcherToRestaurant
    {
        [Required]
        public Guid DispatcherId { get; set; }

        [ForeignKey(nameof(DispatcherId))]
        public ApplicationUser Dispatcher { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public Restaurant Restaurant { get; set; }
    }
}
