using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    //TODO: export to appsettings.json
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddScoped<IOwnerApplicationService, OwnerApplicationService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IDispatcherService, DispatcherService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //seed dev data
                await app.SeedDefaultAccountsAsync();

                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Admin}/{action=Pending}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });

            app.MapRazorPages();

            app.Run();
        }
    }
}

//TODO: Manage:email check for existing email
//TODO: Manage:phoneNumber check for existing phoneNumber