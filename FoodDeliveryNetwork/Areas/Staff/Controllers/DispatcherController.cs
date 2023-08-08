using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.ViewModels.Dispatcher;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class DispatcherController : Controller
    {
        private readonly IDispatcherService dispatcherService;
        private readonly IOrderService orderService;
        public DispatcherController(IDispatcherService dispatcherService, IOrderService orderService)
        {
            this.dispatcherService = dispatcherService;
            this.orderService = orderService;

        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AllOrdersViewModel model)
        {
            Guid currentRestaurant = await dispatcherService.GetRestaurantIdByDispatcherId(User.GetId());
            if (currentRestaurant == Guid.Empty)
                //return RedirectToAction("Index", "Home", new { area = "" }); //not sure for now
                return View(new AllOrdersViewModel());

            model = await orderService.GetAllActiveOrdersByRestaurantId(currentRestaurant, model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(DispacherChangeStatusModel model)
        {
            if (model is null || model.OrderId == Guid.Empty)
            {
                return RedirectToAction("Index", "Dispatcher", new { area = "Staff" });
            }

            bool hasAccess = await orderService.OrderCanBeAccessedByDispatcher(model.OrderId, User.GetId());
            if (!hasAccess)
            {
                return RedirectToAction("Index", "Dispatcher", new { area = "Staff" });
            }

            if (model.NewStatus == Data.Models.OrderStatus.CancelledByRestaurant)
            {
                bool canCancel = await orderService.OrderCanBeCancelledByDispatcher(model.OrderId);
                if (canCancel)
                {
                    await orderService.ChangeOrderStatus(model.OrderId, model.NewStatus);
                }
            }
            else
            {
                bool canChange = await orderService.OrderStatusCanBeChangedByDispatcher(model.OrderId, model.NewStatus);
                if (canChange)
                {
                    await orderService.ChangeOrderStatus(model.OrderId, model.NewStatus);
                }
            }

            return RedirectToAction("Index", "Dispatcher", new { area = "Staff" });
        }

        [HttpGet]
        public async Task<IActionResult> Archived([FromQuery] AllOrdersViewModel model)
        {
            Guid currentRestaurant = await dispatcherService.GetRestaurantIdByDispatcherId(User.GetId());
            if (currentRestaurant == Guid.Empty)
                //return RedirectToAction("Index", "Home", new { area = "" }); //not sure for now
                return View(new AllOrdersViewModel());

            model = await orderService.GetAllArchivedOrdersByRestaurantId(currentRestaurant, model);

            return View(model);
        }
    }
}
