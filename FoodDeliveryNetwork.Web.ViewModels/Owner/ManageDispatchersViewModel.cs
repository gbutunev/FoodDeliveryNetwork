using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class ManageDispatchersViewModel
    {
        [EmailAddress]
        [Display(Name = "New dispatcher's email address")]
        public string NewDispatcherEmail { get; set; }
        public Guid DispatcherIdToBeDeleted { get; set; }
        public List<StaffViewModel> Dispatchers { get; set; }
    }
}
