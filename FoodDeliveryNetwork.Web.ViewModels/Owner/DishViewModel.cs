﻿namespace FoodDeliveryNetwork.Web.ViewModels.Owner
{
    public class DishViewModel
    {
        public int DishId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RestaurantId { get; set; }
        public decimal Price { get; set; }

        public string ImageGuid { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
    }
}
