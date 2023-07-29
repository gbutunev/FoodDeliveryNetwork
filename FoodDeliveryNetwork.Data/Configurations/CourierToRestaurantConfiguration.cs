using FoodDeliveryNetwork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDeliveryNetwork.Data.Configurations
{
    public class CourierToRestaurantConfiguration : IEntityTypeConfiguration<CourierToRestaurant>
    {
        public void Configure(EntityTypeBuilder<CourierToRestaurant> builder)
        {
            builder.HasKey(x => new { x.CourierId, x.RestaurantId });

            builder.HasOne(x => x.Courier)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
