﻿using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Data.Models
{
    public class RestaurantOwner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.EIKMaxLength)]
        public string EIK { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.CompanyNameMaxLength)]
        public string CompanyName { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.FullNameMaxLength)]
        public string OwnerFullName { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.EGNLength)]
        public string OwnerEGN { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.HeadquartersFullAddressMaxLength)]
        public string HeadquartersFullAddress { get; set; }
    }
}
