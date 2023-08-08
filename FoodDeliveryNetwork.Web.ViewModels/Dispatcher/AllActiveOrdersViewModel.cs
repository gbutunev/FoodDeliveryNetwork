using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Dispatcher
{
    public class AllActiveOrdersViewModel
    {
        public BaseQueryModel BaseQueryModel { get; set; } = new();
        public int TotalOrders { get; set; }
        public IEnumerable<SingleActiveOrderViewModel> Orders { get; set; } = new HashSet<SingleActiveOrderViewModel>();
    }
}
