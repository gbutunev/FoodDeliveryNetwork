using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerAllOrdersViewModel
    {
        public BaseQueryModel BaseQueryModel { get; set; } = new BaseQueryModel();
        public int TotalOrders { get; set; }
        public IEnumerable<CustomerBasicOrderViewModel> Orders { get; set; } = new HashSet<CustomerBasicOrderViewModel>();
    }
}
