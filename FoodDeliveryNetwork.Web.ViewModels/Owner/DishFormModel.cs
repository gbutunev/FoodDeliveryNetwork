using FoodDeliveryNetwork.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FoodDeliveryNetwork.Web.ViewModels.Common.CustomAttributes;
using Microsoft.AspNetCore.Http;

namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class DishFormModel
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

        //Not required
        [DataType(DataType.Upload)]
        [Display(Name = "Dish image")]
        //FileExtension Attribute only works on string properties !!!!!!!!!
        //[FileExtensions(Extensions = "jpg,jpeg,png,JPG,JPEG,PNG", ErrorMessage = "Please select a JPG or PNG file.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "The file size should not exceed 5MB.")]
        public IFormFile Image { get; set; }

        public string ImageGuid { get; set; }
    }
}
