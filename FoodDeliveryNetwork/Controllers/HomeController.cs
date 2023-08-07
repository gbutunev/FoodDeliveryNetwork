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
        public HomeController(IRestaurantService restaurantService, IOrderService orderService)
        {
            this.restaurantService = restaurantService;
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CustomerAllRestaurantsViewModel model)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}