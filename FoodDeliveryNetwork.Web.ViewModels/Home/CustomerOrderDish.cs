namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerOrderDish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string ImageGuid { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
    }
}
