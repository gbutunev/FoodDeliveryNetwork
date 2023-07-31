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
        private readonly IDispatcherService dispatcherService;
        public OwnerController(IRestaurantService restaurantService, IDispatcherService dispatcherService)
        {
            this.restaurantService = restaurantService;
            this.dispatcherService = dispatcherService;
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

        [HttpGet]
        public async Task<IActionResult> EditRestaurant(string id)
        {
            var userId = User.GetId();

            bool userIsOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, userId);

            if (userIsOwner)
            {
                RestaurantFormModel restaurant = await restaurantService.GetRestaurantByIdAsync(Guid.Parse(id));

                return View(restaurant);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRestaurant(string id, RestaurantFormModel model)
        {
            int r = await restaurantService.EditRestaurantAsync(id, model);

            if (r == 1)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                switch (r)
                {
                    case -4:
                        ModelState.AddModelError(nameof(model.Name), "Restaurant with this name already exists.");
                        return View(model);
                    case -3:
                        ModelState.AddModelError(nameof(model.Handle), "Restaurant with this handle already exists.");
                        return View(model);
                    case -2:
                        ModelState.AddModelError("", "User does not have required permissions!");
                        return View(model);
                    case -1:
                    case 0:
                        ModelState.AddModelError("", "Something went wrong!");
                        return View(model);
                    default:
                        throw new Exception("Unknown error!");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageDispatchers(string id)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            List<StaffViewModel> staff = (await dispatcherService.GetDispatchersByRestaurantIdAsync(id)).ToList();

            ManageDispatchersViewModel model = new ManageDispatchersViewModel
            {
                Dispatchers = staff
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageDispatchers(string id, ManageDispatchersViewModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            bool invalidModelState = !ModelState.IsValid;
            bool noChanges = string.IsNullOrEmpty(model.NewDispatcherEmail) && model.DispatcherIdToBeDeleted == Guid.Empty;

            if (invalidModelState || noChanges)
            {
                if (noChanges)
                {
                    ModelState.AddModelError("", "Please enter an email address.");
                }

                //same as GET
                if (!isOwner) return RedirectToAction(nameof(Index));

                model.Dispatchers = (await dispatcherService.GetDispatchersByRestaurantIdAsync(id)).ToList();

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewDispatcherEmail))
            {
                int r = await dispatcherService.AddDispatcherToRestaurantAsync(id, model.NewDispatcherEmail);

                switch (r)
                {
                    case 1:
                        //TODO: Add success message
                        break;
                    case 0:
                    case -1:
                        ModelState.AddModelError("", "Something went wrong!");
                        break;
                    case -2:
                        ModelState.AddModelError("", "User does with this email does not exist!");
                        break;
                    case -3:
                        ModelState.AddModelError("", "User is already in a role!");
                        break;
                    case -4:
                        ModelState.AddModelError("", "User is already a dispatcher!");
                        break;
                    default:
                        throw new NotImplementedException("Unknown error!");
                }

                model.Dispatchers = (await dispatcherService.GetDispatchersByRestaurantIdAsync(id)).ToList();
                return View(model);
            }

            if (model.DispatcherIdToBeDeleted != Guid.Empty)
            {
                int r = await dispatcherService.RemoveDispatcherFromRestaurantAsync(id, model.DispatcherIdToBeDeleted);

                switch (r)
                {
                    case 1:
                        //TODO: Add success message
                        break;
                    case 0:
                    case -1:
                        ModelState.AddModelError("", "Something went wrong!");
                        break;
                    case -2:
                        ModelState.AddModelError("", "User does not exist!");
                        break;
                    case -3:
                        ModelState.AddModelError("", "User is not a dispatcher!");
                        break;
                    default:
                        throw new NotImplementedException("Unknown error!");
                }
            }

            //same as GET
            model.Dispatchers = (await dispatcherService.GetDispatchersByRestaurantIdAsync(id)).ToList();
            return View(model);
        }
    }
}
