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

            //builder.OwnsMany(x => x.Dishes, ownedNavigationBuilder =>
            //{
            //    ownedNavigationBuilder.Property<string>("Dishes")
            //         .HasColumnName("Dishes")
            //         .HasConversion(
            //             dishes => JsonSerializer.Serialize(dishes),
            //             json => JsonSerializer.Deserialize<IEnumerable<OrderDish>>(json, null)
            //         );
            //});

            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
