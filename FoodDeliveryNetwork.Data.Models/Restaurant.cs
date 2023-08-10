using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class Restaurant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.NameMaxLength)]
        public string Handle { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(EntityConstants.RestaurantConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.AddressMaxLength)]
        public string Address { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        [ForeignKey(nameof(OwnerId))]
        public ApplicationUser Owner { get; set; }

        public string ImageGuid { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; } = new HashSet<Dish>();
        public virtual ICollection<DispatcherToRestaurant> Dispatchers { get; set; } = new HashSet<DispatcherToRestaurant>();
        public virtual ICollection<CourierToRestaurant> Couriers { get; set; } = new HashSet<CourierToRestaurant>();
    }
}
