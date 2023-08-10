using FoodDeliveryNetwork.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodDeliveryNetwork.Web.Filters
{
    public class RoleRedirectFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.IsInRole(AppConstants.RoleNames.OwnerRole))
            {
                context.Result = new RedirectToActionResult("Index", "Owner", new { area = "" });
            }
            else if (user.IsInRole(AppConstants.RoleNames.AdministratorRole))
            {
                context.Result = new RedirectToActionResult("Pending", "Admin", new { area = "Admin" });
            }
            else if (user.IsInRole(AppConstants.RoleNames.CourierRole))
            {
                context.Result = new RedirectToActionResult("Index", "Courier", new { area = "Staff" });
            }
            else if (user.IsInRole(AppConstants.RoleNames.DispatcherRole))
            {
                context.Result = new RedirectToActionResult("Index", "Dispatcher", new { area = "Staff" });
            }
        }
    }
}
