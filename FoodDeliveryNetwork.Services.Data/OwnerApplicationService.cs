using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryNetwork.Services.Data
{
    public class OwnerApplicationService : IOwnerApplicationService
    {
        private readonly ApplicationDbContext dbContext;
        public OwnerApplicationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddOwnerApplicationAsync(OwnerApplication ownerApplication)
        {
            await dbContext.OwnerApplications.AddAsync(ownerApplication);
            await dbContext.SaveChangesAsync();
        }

        public async Task<AccessToApplicationPage> CheckOwnerStatus(Guid userId)
        {
            var lastApplication = await dbContext.OwnerApplications
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefaultAsync();

            if (lastApplication is null || lastApplication.ApplicationStatus == OwnerApplicationStatus.Rejected)
                return AccessToApplicationPage.CanApply;

            if (lastApplication.ApplicationStatus == OwnerApplicationStatus.Pending)
                return AccessToApplicationPage.Pending;

            return AccessToApplicationPage.Accepted;
        }

        //public async Task<IEnumerable<OwnerApplication>> GetAllPendingApplicationsAsync(BaseQueryModel query)
        //{
        //    if (query is null)
        //        return await dbContext.OwnerApplications
        //            .Where(x => x.ApplicationStatus == OwnerApplicationStatus.Pending)
        //            .OrderByDescending(x => x.CreatedOn)
        //            .ToArrayAsync();

        //    var pendingApplicationsQuery = dbContext
        //        .OwnerApplications
        //        .Where(x => x.ApplicationStatus == OwnerApplicationStatus.Pending)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        //    {
        //        var wildcard = $"%{query.SearchTerm.ToLower()}%";

        //        pendingApplicationsQuery = pendingApplicationsQuery
        //            .Where(x => EF.Functions.Like(x.OwnerFullName.ToLower(), wildcard) ||
        //                        EF.Functions.Like(x.OwnerEGN.ToLower(), wildcard) ||
        //                        EF.Functions.Like(x.CompanyName.ToLower(), wildcard) ||
        //                        EF.Functions.Like(x.EIK.ToLower(), wildcard) ||
        //                        EF.Functions.Like(x.HeadquartersFullAddress, wildcard));
        //    }

        //    switch (query.SortBy)
        //    {
        //        case BaseQueryModelSort.Newest:
        //            pendingApplicationsQuery = pendingApplicationsQuery.OrderByDescending(x => x.CreatedOn);
        //            break;
        //        case BaseQueryModelSort.Oldest:
        //            pendingApplicationsQuery = pendingApplicationsQuery.OrderBy(x => x.CreatedOn);
        //            break;                
        //        default:
        //            pendingApplicationsQuery = pendingApplicationsQuery.OrderByDescending(x => x.CreatedOn);
        //            break;
        //    }

        //    IEnumerable<OwnerApplication> applications = await pendingApplicationsQuery
        //        .Skip((query.Page - 1) * query.PageSize)
        //        .Take(query.PageSize)
        //        .ToArrayAsync();

        //    int totalApplications = applications.Count();

        //    return new AllHousesFilteredAndPagedServiceModel()
        //    {
        //        TotalHousesCount = totalHouses,
        //        Houses = allHouses
        //    };
        //}
    }
}
