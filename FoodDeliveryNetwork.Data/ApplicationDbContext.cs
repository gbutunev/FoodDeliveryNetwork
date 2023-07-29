using FoodDeliveryNetwork.Data.Configurations;
using FoodDeliveryNetwork.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CourierToRestaurantConfiguration());
            builder.ApplyConfiguration(new DispatcherToRestaurantConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());

            base.OnModelCreating(builder);
        }
    }
}