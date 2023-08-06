using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Models;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodDeliveryNetwork.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestaurantService restaurantService;
        public HomeController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AllRestaurantsViewModel model)
        {
            var newModel = await restaurantService.GetAllRestaurantsAsync(model);

            return View(newModel);
        }

        [HttpGet]
        public async Task<IActionResult> Restaurant(string id)
        {
            var restaurant = await restaurantService.GetRestaurantByHandleAsync(id);

            if (restaurant is null) return RedirectToAction(nameof(Index));

            return View(restaurant);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}