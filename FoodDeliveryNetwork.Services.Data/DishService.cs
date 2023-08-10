using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class DishService : IDishService
    {
        private readonly ApplicationDbContext dbContext;
        public DishService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> AddDishToRestaurantAsync(DishFormModel model)
        {
            try
            {
                Dish dish = new Dish
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    RestaurantId = model.RestaurantId,
                    ImageGuid = model.ImageGuid
                };

                await dbContext.Dishes.AddAsync(dish);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<DishViewModel>> GetOwnerDishesByRestaurantIdAsync(string id)
        {
            return await dbContext
                .Dishes
                .Where(d => d.RestaurantId.ToString() == id)
                .Select(d => new DishViewModel
                {
                    DishId = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description,
                    RestaurantId = d.RestaurantId,
                    ImageGuid = d.ImageGuid,
                })
                .ToListAsync();
        }

        public async Task<DishFormModel> GetDishByIdAsync(int dishId)
        {
            return await dbContext
                .Dishes
                .Where(d => d.Id == dishId)
                .Select(d => new DishFormModel
                {
                    DishId = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description,
                    RestaurantId = d.RestaurantId,
                    ImageGuid = d.ImageGuid
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteDishAsync(DishFormModel model)
        {
            var dishToDelete = await dbContext.Dishes.FirstOrDefaultAsync(d => d.Id == model.DishId);

            if (dishToDelete is null || dishToDelete.RestaurantId != model.RestaurantId) return -1;

            try
            {
                dbContext.Dishes.Remove(dishToDelete);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> EditDishAsync(DishFormModel model)
        {
            var dishToEdit = await dbContext.Dishes.FirstOrDefaultAsync(d => d.Id == model.DishId);

            if (dishToEdit is null || dishToEdit.RestaurantId != model.RestaurantId)
                return -1;

            try
            {
                dishToEdit.Name = model.Name;
                dishToEdit.Description = model.Description;
                dishToEdit.Price = model.Price;
                dishToEdit.ImageGuid = model.ImageGuid;

                dbContext.Dishes.Update(dishToEdit);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public async Task<IEnumerable<CustomerOrderDish>> GetCustomerDishesByRestaurantIdAsync(string id)
        {
            return await dbContext
                .Dishes
                .Where(d => d.RestaurantId.ToString() == id)
                .Select(d => new CustomerOrderDish
                {
                    Id = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description,
                    ImageGuid = d.ImageGuid,
                })
                .ToListAsync();
        }
    }
}
