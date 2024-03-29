﻿namespace FoodDeliveryNetwork.Web.ViewModels.Home
{
    public class CustomerRestaurantViewModel
    {
        public Guid Id { get; set; }
        public string Handle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string ImageGuid { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
    }
}