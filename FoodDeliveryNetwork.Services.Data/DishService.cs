using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
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

        public async Task<int> AddDishToRestaurantAsync(DishViewModel model)
        {
            try
            {
                Dish dish = new Dish
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    RestaurantId = model.RestaurantId
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

        public async Task<IEnumerable<DishViewModel>> GetDishesByRestaurantIdAsync(string id)
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
                })
                .ToListAsync();
        }

        public async Task<DishViewModel> GetDishByIdAsync(int dishId)
        {
            return await dbContext
                .Dishes
                .Where(d => d.Id == dishId)
                .Select(d => new DishViewModel
                {
                    DishId = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description,
                    RestaurantId = d.RestaurantId,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteDishAsync(DishViewModel model)
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

        public async Task<int> EditDishAsync(DishViewModel model)
        {
            var dishToEdit = await dbContext.Dishes.FirstOrDefaultAsync(d => d.Id == model.DishId);

            if (dishToEdit is null || dishToEdit.RestaurantId != model.RestaurantId)
                return -1;

            try
            {
                dishToEdit.Name = model.Name;
                dishToEdit.Description = model.Description;
                dishToEdit.Price = model.Price;

                dbContext.Dishes.Update(dishToEdit);

                await dbContext.SaveChangesAsync();

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
