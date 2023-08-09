using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.ViewModels.Courier;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class CourierController : Controller
    {
        private readonly IDeliveryService deliveryService;
        private readonly IOrderService orderService;
        public CourierController(IDeliveryService deliveryService, IOrderService orderService)
        {
            this.deliveryService = deliveryService;
            this.orderService = orderService;

        }

        public async Task<IActionResult> Index([FromQuery] AllOrdersViewModel model)
        {
            var r = await deliveryService.GetAllAvailableOrdersByUserId(User.GetId(), model);
            return View(r);
        }
        public async Task<IActionResult> Assigned([FromQuery] AllOrdersViewModel model)
        {
            var r = await deliveryService.GetAllAssignedOrdersByUserId(User.GetId(), model);
            return View(r);
        }
        public async Task<IActionResult> Archived([FromQuery] AllOrdersViewModel model)
        {
            var r = await deliveryService.GetAllArchivedOrdersByUserId(User.GetId(), model);
            return View(r);
        }

        public async Task<IActionResult> ManageOrder(Guid orderId, OrderStatus newStatus)
        {
            bool hasAccess = await orderService.OrderCanBeAccessedByCourier(orderId, User.GetId());
            if (!hasAccess)
            {
                return RedirectToAction("Assigned", "Courier", new { area = "Staff" });
            }

            bool canChange = await orderService.OrderStatusCanBeChangedByCourier(orderId, newStatus);
            if (canChange)
            {
                if (newStatus == OrderStatus.OnTheWay)
                {
                    int r = await orderService.AssignCourierToOrder(User.GetId(), orderId);
                }

                int r2 = await orderService.ChangeOrderStatus(orderId, newStatus);
            }

            return RedirectToAction("Assigned", "Courier", new { area = "Staff" });
        }
    }
}
