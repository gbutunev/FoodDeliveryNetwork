using FoodDeliveryNetwork.Web.ViewModels.Common;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerAllRestaurantsViewModel : BaseQueryModel
    {
        public int TotalRestaurants { get; set; }
        public IEnumerable<CustomerRestaurantViewModel> Restaurants { get; set; } = new HashSet<CustomerRestaurantViewModel>();
    }
}
