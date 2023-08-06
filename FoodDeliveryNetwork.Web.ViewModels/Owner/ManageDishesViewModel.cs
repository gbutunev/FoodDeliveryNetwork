namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class ManageDishesViewModel
    {
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<DishViewModel> Dishes { get; set; }
    }
}
