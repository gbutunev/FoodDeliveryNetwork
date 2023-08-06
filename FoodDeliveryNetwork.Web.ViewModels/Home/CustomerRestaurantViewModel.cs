namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerRestaurantViewModel
    {
        public Guid Id { get; set; }
        public string Handle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}