using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Web.ViewModels.Common.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class RestaurantFormModel
    {
        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.NameMaxLength)]
        [MinLength(EntityConstants.RestaurantConstants.NameMinLength)]
        [Display(Name = "URL handle")]
        public string Handle { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.NameMaxLength)]
        [MinLength(EntityConstants.RestaurantConstants.NameMinLength)]
        public string Name { get; set; }

        [MaxLength(EntityConstants.RestaurantConstants.DescriptionMaxLength)]
        [MinLength(EntityConstants.RestaurantConstants.DescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.AddressMaxLength)]
        [MinLength(EntityConstants.RestaurantConstants.AddressMinLength)]
        public string Address { get; set; }

        [Required]
        [MaxLength(EntityConstants.RestaurantConstants.PhoneNumberMaxLength)]
        [MinLength(EntityConstants.RestaurantConstants.PhoneNumberMinLength)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string OwnerId { get; set; }

        //Not required
        [DataType(DataType.Upload)]
        [Display(Name = "Background image")]
        //FileExtension Attribute only works on string properties !!!!!!!!!
        //[FileExtensions(Extensions = "jpg,jpeg,png,JPG,JPEG,PNG", ErrorMessage = "Please select a JPG or PNG file.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "The file size should not exceed 5MB.")]
        public IFormFile Image { get; set; }

        public string ImageGuid { get; set; }
    }
}
