namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class RestaurantViewModel
    {
        public Guid Id { get; set; }
        public string Handle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalDishes { get; set; }
        public int TotalDispatchers { get; set; }
        public int TotalCouriers { get; set; }        
    }
}
