namespace FoodDeliveryNetwork.Data.Models
{
    public enum OrderStatus
    {
        Pending,
        Cooking,
        ReadyForPickup,
        OnTheWay,
        Delivered,
        CancelledByCustomer,
        CancelledByRestaurant,
        ReturnedToRestaurant,
    }
}
