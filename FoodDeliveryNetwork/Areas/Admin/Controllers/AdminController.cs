﻿using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryNetwork.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private IOwnerApplicationService ownerApplicationService;
        public AdminController(IOwnerApplicationService ownerApplicationService)
        {
            this.ownerApplicationService = ownerApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Archived([FromQuery] AllApplicationsViewModel viewModel)
        {
            var newModel = await ownerApplicationService.GetAllApplicationsAsync(viewModel, true);

            return View(newModel);
        }

        [HttpGet]
        public async Task<IActionResult> Pending([FromQuery] AllApplicationsViewModel viewModel)
        {
            var newModel = await ownerApplicationService.GetAllApplicationsAsync(viewModel, false);

            return View(newModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await ownerApplicationService.GetApplicationByIdAsync(id);

            if (model is null) return RedirectToAction(nameof(Pending));
            if (model.ApplicationStatus != OwnerApplicationStatus.Pending) return RedirectToAction(nameof(Archived));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details([FromForm] ApplicationChangeStatusViewModel model)
        {

            int r = await ownerApplicationService.ChangeApplicationStatusAsync(model.Id, model.NewStatus);

            //TempData["result"] = r;

            return Redirect(nameof(Pending));
        }
    }
}
