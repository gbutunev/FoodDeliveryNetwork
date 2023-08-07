using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Areas.Staff.Controllers
{
    public class CourierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
