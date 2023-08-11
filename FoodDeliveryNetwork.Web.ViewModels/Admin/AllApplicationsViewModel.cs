using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Admin
{
    public class AllApplicationsViewModel : BaseQueryModel
    {
        public int TotalApplications { get; set; }
        public IEnumerable<SingleApplicationViewModel> Applications { get; set; } = new HashSet<SingleApplicationViewModel>();
    }
}
