using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.Models;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodDeliveryNetwork.Web.Controllers
{
    public class HomeController : Controller
    {
        private const int RECENT_ORDERS_HOUR = 6;

        private readonly IRestaurantService restaurantService;
        private readonly IOrderService orderService;
        private readonly IAddressService addressService;
        private readonly IPictureService pictureService;
        public HomeController(IRestaurantService restaurantService, IOrderService orderService, IAddressService addressService, IPictureService pictureService)
        {
            this.restaurantService = restaurantService;
            this.orderService = orderService;
            this.addressService = addressService;
            this.pictureService = pictureService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CustomerAllRestaurantsViewModel model)
        {
            var newModel = await restaurantService.GetAllRestaurantsAsync(model);

            foreach (var item in newModel.Restaurants.Where(x => x.ImageGuid is not null))
            {
                var pic = await pictureService.GetImage(item.ImageGuid);
                item.Image = pic.Item1;
                item.ImageType = pic.Item2;
            }

            return View(newModel);
        }

        [HttpGet]
        public async Task<IActionResult> Restaurant(string id)
        {
            var restaurant = await restaurantService.GetRestaurantByHandleAsync(id);

            if (restaurant is null) return RedirectToAction(nameof(Index));

            return View(restaurant);
        }

        [HttpGet]
        public async Task<IActionResult> MyRecentOrders([FromQuery] CustomerAllOrdersViewModel model)
        {
            bool hasRecentOrders = await orderService.UserHasRecentOrders(User.GetId(), RECENT_ORDERS_HOUR);
            if (!hasRecentOrders) return RedirectToAction(nameof(MyOrders));

            var orders = await orderService.GetOrdersByCustomerId(model, User.GetId(), RECENT_ORDERS_HOUR);

            return View(orders);
        }

        public async Task<IActionResult> MyOrders([FromQuery] CustomerAllOrdersViewModel model)
        {
            var orders = await orderService.GetOrdersByCustomerId(model, User.GetId());

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(CustomerBasicOrderViewModel model)
        {
            bool orderCanBeCancelled = await orderService.OrderCanBeCancelledByUser(model.Id, User.GetId());

            if (!orderCanBeCancelled) return RedirectToAction(nameof(MyRecentOrders));

            int r = await orderService.ChangeOrderStatus(model.Id, OrderStatus.CancelledByCustomer);

            //TODO: Add notification

            return RedirectToAction(nameof(MyRecentOrders));
        }

        [HttpGet]
        public async Task<IActionResult> MyAddresses()
        {
            var addresses = await addressService.GetAddressesByUserId(User.GetId());

            AddressPageViewModel model = new AddressPageViewModel
            {
                Addresses = addresses
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MyAddresses(AddressPageViewModel model)
        {
            IEnumerable<AddressViewModel> addresses;

            if (!ModelState.IsValid)
            {
                addresses = await addressService.GetAddressesByUserId(User.GetId());
                model.Addresses = addresses;
                return View(model);
            }

            if (model.AddressIdToBeDeleted is null && string.IsNullOrWhiteSpace(model.NewAddress))
            {
                addresses = await addressService.GetAddressesByUserId(User.GetId());
                model.Addresses = addresses;
                return View(model);
            }

            if (model.AddressIdToBeDeleted is not null && model.AddressIdToBeDeleted > 0)
            {
                bool userOwnsAddress = await addressService.UserOwnsAddress(User.GetId(), model.AddressIdToBeDeleted.Value);
                if (userOwnsAddress)
                {
                    await addressService.DeleteAddressById(model.AddressIdToBeDeleted.Value);
                }

                addresses = await addressService.GetAddressesByUserId(User.GetId());
                model.Addresses = addresses;
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewAddress))
            {
                CustomerAddress customerAddress = new()
                {
                    Address = model.NewAddress.Trim(),
                    CustomerId = Guid.Parse(User.GetId())
                };

                int r = await addressService.CreateAddress(customerAddress);

                //TODO: Add notification
                switch (r)
                {
                    case 1:
                        addresses = await addressService.GetAddressesByUserId(User.GetId());
                        model.Addresses = addresses;
                        return View(model);
                    default:
                        break;
                }
            }

            addresses = await addressService.GetAddressesByUserId(User.GetId());
            model.Addresses = addresses;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(MyRecentOrders));

            bool userOwnsOrder = await orderService.UserOwnsOrder(User.GetId(), id);
            if (!userOwnsOrder) return RedirectToAction(nameof(MyRecentOrders));

            var order = await orderService.GetOrderById(id);

            if (order is null) return RedirectToAction(nameof(Index));

            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}