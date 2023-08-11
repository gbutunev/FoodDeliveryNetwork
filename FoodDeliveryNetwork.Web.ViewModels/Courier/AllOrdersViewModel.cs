using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Courier
{
    public class AllOrdersViewModel : BaseQueryModel
    {
        public int TotalOrders { get; set; }
        public IEnumerable<SingleOrderViewModel> Orders { get; set; } = new HashSet<SingleOrderViewModel>();
    }
}
