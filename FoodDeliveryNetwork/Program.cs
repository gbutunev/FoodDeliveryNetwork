using Blazored.LocalStorage;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.SignalR;
using FoodDeliveryNetwork.Web.Binders;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.Filters;
using FoodDeliveryNetwork.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
                options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
            {
                o.DetailedErrors = builder.Environment.IsDevelopment();
            });

            //builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IOwnerApplicationService, OwnerApplicationService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IDispatcherService, DispatcherService>();
            builder.Services.AddScoped<ICourierService, CourierService>();
            builder.Services.AddScoped<IDishService, DishService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<IPictureService, PictureService>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddSignalR();

            builder.Services.AddAuthentication()
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                });

            var app = builder.Build();

            await app.SeedDefaultRoles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //seed dev data
                await app.SeedDefaultAccountsAsync();

                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error/500");
                app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
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

                endpoints.MapBlazorHub();

                app.MapHub<OrderUpdateHub>("/livedelivery");
            });

            app.MapRazorPages();

            app.Run();
        }
    }
}

//TODO: Manage:email check for existing email
//TODO: Manage:phoneNumber check for existing phoneNumber