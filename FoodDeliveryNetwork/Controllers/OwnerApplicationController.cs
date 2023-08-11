using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.Extensions;
using FoodDeliveryNetwork.Web.ViewModels.OwnerApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Controllers
{
    [Authorize]
    public class OwnerApplicationController : Controller
    {
        private readonly IOwnerApplicationService ownerApplicationService;

        public OwnerApplicationController(IOwnerApplicationService ownerApplicationService)
        {
            this.ownerApplicationService = ownerApplicationService;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.GetId();

            var status = await ownerApplicationService.CheckOwnerStatus(userId);

            OwnerApplicationViewModel viewModel = new OwnerApplicationViewModel
            {
                AccessToApplicationPage = status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(OwnerApplicationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            string userId = User.GetId();

            var status = await ownerApplicationService.CheckOwnerStatus(userId);

            if (status != AccessToApplicationPage.CanApply)
            {
                viewModel.AccessToApplicationPage = status;
                return View(viewModel);
            }

            try
            {
                OwnerApplication ownerApplication = new OwnerApplication
                {
                    ApplicationUserId = Guid.Parse(userId),
                    OwnerFullName = viewModel.OwnerFullName,
                    OwnerEGN = viewModel.OwnerEGN,
                    CompanyName = viewModel.CompanyName,
                    EIK = viewModel.EIK,
                    HeadquartersFullAddress = viewModel.HeadquartersFullAddress,
                    ApplicationStatus = OwnerApplicationStatus.Pending,
                    CreatedOn = DateTime.Now
                };

                await ownerApplicationService.AddOwnerApplicationAsync(ownerApplication);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "There was an unexpected error. Please try again or contact an administrator if the problem persists");
                return View(viewModel);
            }

            TempData[AppConstants.NotificationTypes.SuccessMessage] = "Application was sent successfully.";

            return RedirectToAction("Index", "Home");
        }
    }
}
