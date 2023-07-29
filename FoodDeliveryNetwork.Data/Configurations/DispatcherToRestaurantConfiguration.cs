using FoodDeliveryNetwork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDeliveryNetwork.Data.Configurations
{
    public class DispatcherToRestaurantConfiguration : IEntityTypeConfiguration<DispatcherToRestaurant>
    {
        public void Configure(EntityTypeBuilder<DispatcherToRestaurant> builder)
        {
            builder.HasKey(x => new { x.DispatcherId, x.RestaurantId });

            builder.HasOne(x => x.Dispatcher)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
