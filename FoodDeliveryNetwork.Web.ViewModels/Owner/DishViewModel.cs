using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class DishViewModel
    {        
        public int DishId { get; set; }

        [Required]
        [MaxLength(EntityConstants.DishConstants.NameMaxLength)]
        [MinLength(EntityConstants.DishConstants.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(EntityConstants.DishConstants.DescriptionMaxLength)]
        [MinLength(EntityConstants.DishConstants.DescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "decimal(7, 2)")]
        [Range(0.00, 10000)] //should be enough range for a black angus ribeye steak?
        public decimal Price { get; set; }
    }
}
