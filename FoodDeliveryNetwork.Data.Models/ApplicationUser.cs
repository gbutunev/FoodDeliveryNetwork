using FoodDeliveryNetwork.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Data.Models
{
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = true)]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
        }

        [Required]
        [PersonalData]
        [MaxLength(EntityConstants.User.NameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [MaxLength(EntityConstants.User.NameMaxLength)]
        public string LastName { get; set; }

        [Required]
        [PersonalData]
        [MaxLength(EntityConstants.User.PhoneNumberMaxLength)]
        public override string PhoneNumber { get; set; }
    }
}
