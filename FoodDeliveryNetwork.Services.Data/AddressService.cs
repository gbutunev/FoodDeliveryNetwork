using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public AddressService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<bool> AddressExistsAsync(string userId, string address)
        {
            return await dbContext.CustomerAddresses
                .AnyAsync(ca => ca.CustomerId.ToString() == userId && ca.Address == address);
        }

        public async Task<int> CreateAddressAsync(string userId, string address)
        {
            bool isValidGuid = Guid.TryParse(userId, out Guid userGuid);
            if (!isValidGuid) return -1;

            bool userExists = await userManager.FindByIdAsync(userId) is not null;
            if (!userExists) return -2;

            CustomerAddress customerAddress = new CustomerAddress
            {
                CustomerId = userGuid,
                Address = address
            };

            try
            {
                await dbContext.CustomerAddresses.AddAsync(customerAddress);
                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<AddressViewModel>> GetAddressesByUserId(string userId)
        {
            return await dbContext.CustomerAddresses
                .Where(ca => ca.CustomerId.ToString() == userId)
                .Select(ca => new AddressViewModel
                {
                    Id = ca.Id,
                    Address = ca.Address
                })
                .ToArrayAsync();
        }


    }
}
