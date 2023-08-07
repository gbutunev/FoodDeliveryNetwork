using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Areas.Staff.Controllers
{
    public class DispatcherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
