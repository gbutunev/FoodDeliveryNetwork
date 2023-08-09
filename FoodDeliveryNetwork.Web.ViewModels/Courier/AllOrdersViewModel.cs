using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Courier
{
    public class AllOrdersViewModel
    {
        public BaseQueryModel BaseQueryModel { get; set; } = new();
        public int TotalOrders { get; set; }
        public IEnumerable<SingleOrderViewModel> Orders { get; set; } = new HashSet<SingleOrderViewModel>();
    }
}
