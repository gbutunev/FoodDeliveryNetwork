using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class AddressPageViewModel
    {
        public IEnumerable<AddressViewModel> Addresses { get; set; }

        [MinLength(EntityConstants.CustomerConstants.AddressMinLength)]
        [MaxLength(EntityConstants.CustomerConstants.AddressMaxLength)]
        public string NewAddress { get; set; }

        public int? AddressIdToBeDeleted { get; set; }
    }
}
