using FoodDeliveryNetwork.Common;
using FoodDeliveryNetwork.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryNetwork.Web.ViewModels.Admin
{
    public class SingleApplicationViewModel
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string EIK { get; set; }
        public string CompanyName { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerEGN { get; set; }
        public string HeadquartersFullAddress { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public OwnerApplicationStatus ApplicationStatus { get; set; } = OwnerApplicationStatus.Pending;
        public DateTime CreatedOn { get; set; }
    }
}
