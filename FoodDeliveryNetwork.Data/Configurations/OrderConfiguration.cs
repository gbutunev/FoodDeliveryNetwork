using FoodDeliveryNetwork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDeliveryNetwork.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(x => x.Dishes, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });

            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
