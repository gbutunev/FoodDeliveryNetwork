using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.OwnerApplication
{
    public class OwnerApplicationViewModel
    {
        public AccessToApplicationPage AccessToApplicationPage { get; set; }

        [Required]
        [Display(Name = "EIK")]
        [MaxLength(EntityConstants.OwnerConstants.EIKMaxLength)]
        [MinLength(EntityConstants.OwnerConstants.EIKMinLength)]
        public string EIK { get; set; }

        [Required]
        [Display(Name = "Company name")]
        [MaxLength(EntityConstants.OwnerConstants.CompanyNameMaxLength)]
        [MinLength(EntityConstants.OwnerConstants.CompanyNameMinLength)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Owner's full name")]
        [MaxLength(EntityConstants.OwnerConstants.FullNameMaxLength)]
        [MinLength(EntityConstants.OwnerConstants.FullNameMinLength)]
        public string OwnerFullName { get; set; }

        [Required]
        [Display(Name = "EGN")]
        [MaxLength(EntityConstants.OwnerConstants.EGNLength)]
        [MinLength(EntityConstants.OwnerConstants.EGNLength)]
        public string OwnerEGN { get; set; }

        [Required]
        [Display(Name = "Company headquarters' full address")]
        [MaxLength(EntityConstants.OwnerConstants.HeadquartersFullAddressMaxLength)]
        [MinLength(EntityConstants.OwnerConstants.HeadquartersFullAddressMinLength)]
        public string HeadquartersFullAddress { get; set; }
    }
}
