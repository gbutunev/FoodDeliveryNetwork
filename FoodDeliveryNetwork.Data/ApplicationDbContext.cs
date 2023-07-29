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

        public DbSet<CourierToRestaurant> CourierToRestaurants { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DispatcherToRestaurant> DispatcherToRestaurants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OwnerApplication> OwnerApplications { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantOwner> RestaurantOwners { get; set; }
    }
}