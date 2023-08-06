using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryNetwork.Data.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [Required]
        [MaxLength(EntityConstants.CustomerConstants.AddressMaxLength)]
        public string Address { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public Restaurant Restaurant { get; set; }

        [Required]
        [Column(TypeName = "decimal(7, 2)")]
        public decimal TotalPrice { get; set; }

        //this will not be in a table, it will be a json string in case the prices of the dishes change
        [Required]
        public IEnumerable<OrderDish> Dishes { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }

    public class OrderDish
    {
        public string DishName { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(7, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
