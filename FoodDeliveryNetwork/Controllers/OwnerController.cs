using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.ViewModels.Owner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Controllers
{
    [Authorize(Roles = AppConstants.RoleNames.OwnerRole)]
    public class OwnerController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly ICourierService courierService;
        private readonly IDispatcherService dispatcherService;
        private readonly IDishService dishService;
        private readonly IPictureService pictureService;
        public OwnerController(IRestaurantService restaurantService, IDispatcherService dispatcherService, IDishService dishService, ICourierService courierService, IPictureService pictureService)
        {
            this.restaurantService = restaurantService;
            this.dispatcherService = dispatcherService;
            this.dishService = dishService;
            this.courierService = courierService;
            this.pictureService = pictureService;

        }

        //MyRestaurants
        public async Task<IActionResult> Index()
        {
            var ownerId = User.GetId();

            var restaurants = await restaurantService.GetRestaurantsByOwnerIdAsync(ownerId);

            foreach (var item in restaurants.Where(x => x.ImageGuid is not null))
            {
                var pic = await pictureService.GetImage(item.ImageGuid);
                item.Image = pic.Item1;
                item.ImageType = pic.Item2;
            }

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

            if (viewModel.Image is not null && viewModel.Image.Length > 0)
            {
                viewModel.ImageGuid = Guid.NewGuid().ToString();

                try
                {
                    Minio.PutObjectResponse hmm = await pictureService.UploadImage(viewModel.Image, viewModel.ImageGuid);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "A problem occurred while uploading the image!");
                    return View(viewModel);
                }
            }

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
            string oldImageGuid = (await restaurantService.GetRestaurantByIdAsync(Guid.Parse(id))).ImageGuid;
            if (model.Image is not null && model.Image.Length > 0)
            {

                model.ImageGuid = Guid.NewGuid().ToString();

                try
                {
                    Minio.PutObjectResponse hmm = await pictureService.UploadImage(model.Image, model.ImageGuid);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "A problem occurred while uploading the new image!");
                    model.ImageGuid = oldImageGuid;
                    return View(model);
                }

                if (oldImageGuid is not null)
                {
                    try
                    {
                        await pictureService.RemoveImage(oldImageGuid);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                model.ImageGuid = oldImageGuid;
            }

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

        [HttpGet]
        public async Task<IActionResult> ManageCouriers(string id)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            List<StaffViewModel> staff = (await courierService.GetCouriersByRestaurantIdAsync(id)).ToList();

            ManageCouriersViewModel model = new ManageCouriersViewModel
            {
                Couriers = staff
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageCouriers(string id, ManageCouriersViewModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            bool invalidModelState = !ModelState.IsValid;
            bool noChanges = string.IsNullOrEmpty(model.NewCourierEmail) && model.CourierIdToBeDeleted == Guid.Empty;

            if (invalidModelState || noChanges)
            {
                if (noChanges)
                {
                    ModelState.AddModelError("", "Please enter an email address.");
                }

                //same as GET
                if (!isOwner) return RedirectToAction(nameof(Index));

                model.Couriers = (await courierService.GetCouriersByRestaurantIdAsync(id)).ToList();

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewCourierEmail))
            {
                int r = await courierService.AddCourierToRestaurantAsync(id, model.NewCourierEmail);

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
                    default:
                        throw new NotImplementedException("Unknown error!");
                }

                model.Couriers = (await courierService.GetCouriersByRestaurantIdAsync(id)).ToList();
                return View(model);
            }

            if (model.CourierIdToBeDeleted != Guid.Empty)
            {
                int r = await courierService.RemoveCourierFromRestaurantAsync(id, model.CourierIdToBeDeleted);

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
                        ModelState.AddModelError("", "User is not a courier!");
                        break;
                    default:
                        throw new NotImplementedException("Unknown error!");
                }
            }

            //same as GET
            model.Couriers = (await courierService.GetCouriersByRestaurantIdAsync(id)).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDishes(string id)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            List<DishViewModel> dishes = (await dishService.GetOwnerDishesByRestaurantIdAsync(id)).ToList();

            ManageDishesViewModel model = new ManageDishesViewModel
            {
                Dishes = dishes,
                RestaurantId = Guid.Parse(id),
            };

            foreach (var item in model.Dishes.Where(x => x.ImageGuid is not null))
            {
                var pic = await pictureService.GetImage(item.ImageGuid);
                item.Image = pic.Item1;
                item.ImageType = pic.Item2;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> NewDish(string id)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            DishFormModel model = new DishFormModel
            {
                RestaurantId = Guid.Parse(id)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewDish(string id, DishFormModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(id, User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.RestaurantId = Guid.Parse(id);

            if (model.Image is not null && model.Image.Length > 0)
            {
                model.ImageGuid = Guid.NewGuid().ToString();

                try
                {
                    Minio.PutObjectResponse resp = await pictureService.UploadImage(model.Image, model.ImageGuid);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "A problem occurred while uploading the image!");
                    return View(model);
                }
            }

            int r = await dishService.AddDishToRestaurantAsync(model);

            switch (r)
            {
                case 1:
                    return RedirectToAction(nameof(ManageDishes), new { id });
                case 0:
                    ModelState.AddModelError("", "Something went wrong!");
                    return View(model);
                default:
                    throw new NotImplementedException("Unknown error!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditDish(DishFormModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(model.RestaurantId.ToString(), User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            var dbDish = await dishService.GetDishByIdAsync(model.DishId);
            if (dbDish is null || dbDish.RestaurantId != model.RestaurantId)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(dbDish);
        }

        [HttpPost]
        public async Task<IActionResult> ЕditDishSubmit(DishFormModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(model.RestaurantId.ToString(), User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            string oldImageGuid = (await dishService.GetDishByIdAsync(model.DishId)).ImageGuid;
            if (model.Image is not null && model.Image.Length > 0)
            {

                model.ImageGuid = Guid.NewGuid().ToString();

                try
                {
                    Minio.PutObjectResponse hmm = await pictureService.UploadImage(model.Image, model.ImageGuid);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "A problem occurred while uploading the new image!");
                    model.ImageGuid = oldImageGuid;
                    return View(model);
                }

                if (oldImageGuid is not null)
                {
                    try
                    {
                        await pictureService.RemoveImage(oldImageGuid);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                model.ImageGuid = oldImageGuid;
            }

            int r = await dishService.EditDishAsync(model);

            //TODO: success/error messages
            switch (r)
            {
                case -1:
                    return RedirectToAction(nameof(ManageDishes), new { id = model.RestaurantId });
                case 0:
                    ModelState.AddModelError("", "Something went wrong!");
                    return View(nameof(EditDish), model);
                case 1:
                    return RedirectToAction(nameof(ManageDishes), new { id = model.RestaurantId });
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDish(DishFormModel model)
        {
            bool isOwner = await restaurantService.RestaurantIsOwnedByUserAsync(model.RestaurantId.ToString(), User.GetId());
            if (!isOwner) return RedirectToAction(nameof(Index));

            int r = await dishService.DeleteDishAsync(model);

            //TODO: success/error messages
            switch (r)
            {
                case 1:
                    return RedirectToAction(nameof(ManageDishes), new { id = model.RestaurantId });
                case 0:
                    return RedirectToAction(nameof(ManageDishes), new { id = model.RestaurantId });
                case -1:
                    return RedirectToAction(nameof(ManageDishes), new { id = model.RestaurantId });
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
