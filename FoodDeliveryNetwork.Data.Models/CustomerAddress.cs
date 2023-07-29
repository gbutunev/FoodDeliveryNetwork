using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class CustomerAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [Required]
        [MaxLength(EntityConstants.CustomerConstants.AddressMaxLength)]
        public string Address { get; set; }
    }
}
