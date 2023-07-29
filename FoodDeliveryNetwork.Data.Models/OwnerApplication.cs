﻿using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class OwnerApplication
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
        [MaxLength(EntityConstants.OwnerConstants.FullNameMaxLength)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(EntityConstants.OwnerConstants.HeadquartersFullAddressMaxLength)]
        public string HeadquartersFullAddress { get; set; }

        [Required]
        public OwnerApplicationStatus ApplicationStatus { get; set; } = OwnerApplicationStatus.Pending;
    }
}
