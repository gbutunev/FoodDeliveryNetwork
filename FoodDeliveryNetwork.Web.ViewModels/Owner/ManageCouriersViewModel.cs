using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class ManageCouriersViewModel
    {
        [EmailAddress]
        [Display(Name = "New courier's email address")]
        public string NewCourierEmail { get; set; }
        public Guid CourierIdToBeDeleted { get; set; }
        public List<StaffViewModel> Couriers { get; set; }
    }
}
