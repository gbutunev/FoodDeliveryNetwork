using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Controllers
{
    public class OwnerApplicationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
