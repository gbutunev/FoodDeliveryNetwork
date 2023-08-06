namespace FoodDeliveryNetwork.Data.Models
{
    public enum OrderStatus
    {
        Pending,
        Cooking,
        OnTheWay,
        Delivered,
        CancelledByCustomer,
        CancelledByRestaurant,
        ReturnedToRestaurant,
    }
}
