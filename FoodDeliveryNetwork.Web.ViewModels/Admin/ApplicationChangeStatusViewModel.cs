using FoodDeliveryNetwork.Data.Models;

namespace FoodDeliveryNetwork.Web.ViewModels.Admin
{
    public class ApplicationChangeStatusViewModel : SingleApplicationViewModel
    {
        public OwnerApplicationStatus? NewStatus { get; set; }
    }
}
