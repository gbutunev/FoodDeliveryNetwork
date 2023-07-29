using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityConstants.DishConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(EntityConstants.DishConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public Restaurant Restaurant { get; set; }

        [Required]
        [Column(TypeName = "decimal(7, 2)")]
        public decimal Price { get; set; }

        //TODO: Add Images
    }
}
