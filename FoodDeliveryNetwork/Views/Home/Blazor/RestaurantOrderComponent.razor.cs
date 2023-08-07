using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Components;

namespace FoodDeliveryNetwork.Web.Views.Home.Blazor
{
    public partial class RestaurantOrderComponent : ComponentBase
    {
        [Parameter]
        public Guid RestaurantId { get; set; }
        [Parameter]
        public string UserId { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IRestaurantService RestaurantService { get; set; }
        [Inject]
        private IAddressService AddressService { get; set; }
        [Inject]
        private IOrderService OrderService { get; set; }

        private Restaurant currentRestaurant = new Restaurant();

        private List<AddressViewModel> customerAddresses = new List<AddressViewModel>();

        private bool showConfirmOrderPopup = false;
        private bool showAddressPopup = false;

        private AddressPopupMode addressPopupMode = AddressPopupMode.Saved;

        private CustomerBlazorOrderViewModel currentOrder = new CustomerBlazorOrderViewModel();

        protected override async Task OnParametersSetAsync()
        {
            if (RestaurantId == Guid.Empty || string.IsNullOrWhiteSpace(UserId))
                NavigationManager.NavigateTo("/");

            currentRestaurant = await RestaurantService.GetRestaurantByIdAsync(RestaurantId.ToString());

            if (currentRestaurant is null || currentRestaurant.Id == Guid.Empty)
                NavigationManager.NavigateTo("/");

            currentOrder.RestaurantId = currentRestaurant.Id;
            currentOrder.UserId = UserId;
        }

        private void AddDish(Dish dish)
        {
            if (currentOrder.OrderItems.ContainsKey(dish))
            {
                currentOrder.OrderItems[dish]++;
            }
            else
            {
                currentOrder.OrderItems.Add(dish, 1);
            }

        }

        private void RemoveDish(Dish dish)
        {
            if (currentOrder.OrderItems.ContainsKey(dish) && currentOrder.OrderItems[dish] > 0)
            {
                currentOrder.OrderItems[dish]--;

                if (currentOrder.OrderItems[dish] <= 0)
                    currentOrder.OrderItems.Remove(dish);
            }
        }

        private void ShowConfirmOrderPopup()
        {
            if (currentOrder.OrderItems.Count == 0)
                return;

            showConfirmOrderPopup = true;
        }

        private void HideConfirmOrderPopup()
        {
            showConfirmOrderPopup = false;
        }

        private async Task ShowAddressPopup()
        {
            customerAddresses = (await AddressService.GetAddressesByUserId(UserId)).ToList();

            showAddressPopup = true;
        }

        private void HideAddressPopup()
        {
            showAddressPopup = false;
            customerAddresses.Clear();
        }

        private async Task ContinueToAddress()
        {
            HideConfirmOrderPopup();
            await ShowAddressPopup();
        }

        private async Task ConfirmOrder()
        {
            currentOrder.Address = currentOrder.Address.Trim();

            //1. save address if new
            bool addressExists = await AddressService.AddressExistsAsync(UserId, currentOrder.Address);
            if (!addressExists)
            {
                int r0 = await AddressService.CreateAddressAsync(UserId, currentOrder.Address);

                if (r0 != 1)
                {
                    //TODO: show error
                    return;
                }
            }

            //2. create order
            Order order = new Order
            {
                CustomerId = Guid.Parse(UserId),
                RestaurantId = currentRestaurant.Id,
                Address = currentOrder.Address,
                Dishes = currentOrder.OrderItems.Select(oi => new OrderDish
                {
                    DishName = oi.Key.Name,
                    Quantity = oi.Value,
                    UnitPrice = oi.Key.Price
                }).ToList(),
                TotalPrice = currentOrder.OrderItems.Sum(oi => oi.Key.Price * oi.Value)
            };

            int r1 = await OrderService.CreateOrder(order);

            //3. show error or navigate to order details

            HideAddressPopup();
        }
    }

    public enum AddressPopupMode
    {
        New,
        Saved,
    }
}
