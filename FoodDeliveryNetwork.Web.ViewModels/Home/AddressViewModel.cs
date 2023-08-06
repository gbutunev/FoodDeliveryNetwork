using FoodDeliveryNetwork.Common;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        public string Address { get; set; }
    }
}
