using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Controllers
{
    [Authorize]
    public class OwnerController : Controller
    {
        private readonly IRestaurantService restaurantService;

        public OwnerController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        //MyRestaurants
        public async Task<IActionResult> Index()
        {
            var ownerId = User.GetId();

            var restaurants = await restaurantService.GetRestaurantsByOwnerIdAsync(ownerId);

            return View(restaurants);
        }

        [HttpGet]
        public IActionResult NewRestaurant()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewRestaurant(RestaurantFormModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            viewModel.OwnerId = User.GetId();

            var r = await restaurantService.AddRestaurant(viewModel);

            if (r == 1)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                switch (r)
                {
                    case -4:
                        ModelState.AddModelError(nameof(viewModel.Name), "Restaurant with this name already exists.");
                        return View(viewModel);
                    case -3:
                        ModelState.AddModelError(nameof(viewModel.Handle), "Restaurant with this handle already exists.");
                        return View(viewModel);
                    case -2:
                        ModelState.AddModelError("", "User does not have required permissions!");
                        return View(viewModel);
                    case -1:
                    case 0:
                        ModelState.AddModelError("", "Something went wrong!");
                        return View(viewModel);
                    default:
                        throw new Exception("Unknown error!");
                }
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRestaurant(Guid restaurantId)
        {
            //TODO: Add error/success messages

            var userId = User.GetId();

            bool userIsOwner = await restaurantService.RestaurantIsOwnedByUserAsync(restaurantId.ToString(), userId);

            if (userIsOwner)
            {
                int r = await restaurantService.DeleteRestaurantAsync(restaurantId);
            }            

            return RedirectToAction(nameof(Index));
        }
    }
}
