using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Admin;
using FoodDeliveryNetwork.Web.ViewModels.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodDeliveryNetwork.Services.Data
{
    public class OwnerApplicationService : IOwnerApplicationService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public OwnerApplicationService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task AddOwnerApplicationAsync(OwnerApplication ownerApplication)
        {
            await dbContext.OwnerApplications.AddAsync(ownerApplication);
            await dbContext.SaveChangesAsync();
        }

        public async Task<int> ChangeApplicationStatusAsync(int id, OwnerApplicationStatus? newStatus)
        {
            var application = await dbContext
                .OwnerApplications
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (application is null || newStatus is null)
                return -1;



            try
            {
                if (newStatus == OwnerApplicationStatus.Rejected)
                {
                    application.ApplicationStatus = OwnerApplicationStatus.Rejected;
                    dbContext.OwnerApplications.Update(application);
                    await dbContext.SaveChangesAsync();

                    return 1;
                }

                if (newStatus == OwnerApplicationStatus.Approved)
                {
                    //0. check if user is already in a role
                    var userRoles = await userManager.GetRolesAsync(application.ApplicationUser);
                    if (userRoles.Count > 0)
                        return -3;

                    //1. Update Application
                    application.ApplicationStatus = OwnerApplicationStatus.Approved;
                    dbContext.OwnerApplications.Update(application);

                    //2. Add owner info to table
                    RestaurantOwner restaurantOwner = new RestaurantOwner()
                    {
                        ApplicationUserId = application.ApplicationUserId,
                        OwnerFullName = application.OwnerFullName,
                        OwnerEGN = application.OwnerEGN,
                        CompanyName = application.CompanyName,
                        EIK = application.EIK,
                        HeadquartersFullAddress = application.HeadquartersFullAddress,
                    };
                    dbContext.RestaurantOwners.Add(restaurantOwner);

                    //3. Add user to role                    
                    await userManager.AddToRoleAsync(application.ApplicationUser, AppConstants.RoleNames.OwnerRole);

                    await dbContext.SaveChangesAsync();

                    return 1;
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return -2;
        }

        public async Task<AccessToApplicationPage> CheckOwnerStatus(string userId)
        {
            var lastApplication = await dbContext.OwnerApplications
                .Where(x => x.ApplicationUserId.ToString() == userId)
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefaultAsync();

            if (lastApplication is null || lastApplication.ApplicationStatus == OwnerApplicationStatus.Rejected)
                return AccessToApplicationPage.CanApply;

            if (lastApplication.ApplicationStatus == OwnerApplicationStatus.Pending)
                return AccessToApplicationPage.Pending;

            return AccessToApplicationPage.Accepted;
        }

        public async Task<AllApplicationsViewModel> GetAllApplicationsAsync(AllApplicationsViewModel model, bool archived)
        {
            Expression<Func<OwnerApplication, bool>> predicate = null;
            if (archived)
                predicate = x => x.ApplicationStatus == OwnerApplicationStatus.Approved || x.ApplicationStatus == OwnerApplicationStatus.Rejected;
            else
                predicate = x => x.ApplicationStatus == OwnerApplicationStatus.Pending;


            if (model is null || model.BaseQueryModel is null)
            {

                model.Applications = await dbContext.OwnerApplications
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOn)
                    .Select(x => new SingleApplicationViewModel()
                    {
                        Id = x.Id,
                        OwnerFullName = x.OwnerFullName,
                        OwnerEGN = x.OwnerEGN,
                        CompanyName = x.CompanyName,
                        EIK = x.EIK,
                        HeadquartersFullAddress = x.HeadquartersFullAddress,
                        ApplicationStatus = x.ApplicationStatus,
                        CreatedOn = x.CreatedOn,
                        OwnerPhoneNumber = x.ApplicationUser.PhoneNumber,
                    })
                    .ToArrayAsync();

                model.TotalApplications = model.Applications.Count();

                return model;
            }

            var query = model.BaseQueryModel;

            var pendingApplicationsQuery = dbContext
                .OwnerApplications
                .Where(predicate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var wildcard = $"%{query.SearchTerm.ToLower()}%";

                pendingApplicationsQuery = pendingApplicationsQuery
                    .Where(x => EF.Functions.Like(x.OwnerFullName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.OwnerEGN.ToLower(), wildcard) ||
                                EF.Functions.Like(x.CompanyName.ToLower(), wildcard) ||
                                EF.Functions.Like(x.EIK.ToLower(), wildcard) ||
                                EF.Functions.Like(x.HeadquartersFullAddress, wildcard));
            }

            switch (query.SortBy)
            {
                case BaseQueryModelSort.Newest:
                    pendingApplicationsQuery = pendingApplicationsQuery.OrderByDescending(x => x.CreatedOn);
                    break;
                case BaseQueryModelSort.Oldest:
                    pendingApplicationsQuery = pendingApplicationsQuery.OrderBy(x => x.CreatedOn);
                    break;
                default:
                    pendingApplicationsQuery = pendingApplicationsQuery.OrderByDescending(x => x.CreatedOn);
                    break;
            }

            IEnumerable<SingleApplicationViewModel> applications = await pendingApplicationsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new SingleApplicationViewModel()
                {
                    Id = x.Id,
                    OwnerFullName = x.OwnerFullName,
                    OwnerEGN = x.OwnerEGN,
                    CompanyName = x.CompanyName,
                    EIK = x.EIK,
                    HeadquartersFullAddress = x.HeadquartersFullAddress,
                    ApplicationStatus = x.ApplicationStatus,
                    CreatedOn = x.CreatedOn,
                    OwnerPhoneNumber = x.ApplicationUser.PhoneNumber,
                })
                .ToArrayAsync();

            int totalApplications = applications.Count();

            model.Applications = applications;
            model.TotalApplications = totalApplications;

            return model;
        }

        public async Task<ApplicationChangeStatusViewModel> GetApplicationByIdAsync(int id)
        {
            var r = await dbContext.OwnerApplications
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => id == x.Id);

            if (r is null) return null;

            return new ApplicationChangeStatusViewModel()
            {
                Id = r.Id,
                ApplicationUser = r.ApplicationUser,
                OwnerFullName = r.OwnerFullName,
                OwnerEGN = r.OwnerEGN,
                CompanyName = r.CompanyName,
                EIK = r.EIK,
                HeadquartersFullAddress = r.HeadquartersFullAddress,
                ApplicationStatus = r.ApplicationStatus,
                CreatedOn = r.CreatedOn,
                OwnerPhoneNumber = r.ApplicationUser.PhoneNumber,
                NewStatus = null,
            };
        }
    }
}
