using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data.Contracts;
using FoodDeliveryNetwork.Web.ViewModels.Home;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;

namespace FoodDeliveryNetwork.Web.Views.Home.Blazor
{
    public partial class RestaurantOrderComponent : ComponentBase, IDisposable
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
        [Inject]
        private IDishService DishService { get; set; }
        [Inject]
        private IPictureService PictureService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }

        private Restaurant currentRestaurant = new();

        private List<AddressViewModel> customerAddresses = new();

        private bool showConfirmOrderPopup = false;
        private bool showAddressPopup = false;
        private bool hasSavedAddresses = false;
        private bool showResumeDialog = false;

        private AddressPopupMode addressPopupMode = AddressPopupMode.New;

        private CustomerBlazorOrderViewModel currentOrder = new();
        private IEnumerable<CustomerOrderDish> restaurantDishes = new HashSet<CustomerOrderDish>();

        private Dictionary<int, int> sessionOrder = new(); //dishId, quantity
        private string sessionStorageKey;

        private readonly CancellationTokenSource tokenSource = new();
        private CancellationToken token;

        protected override async Task OnParametersSetAsync()
        {
            token = tokenSource.Token;

            if (RestaurantId == Guid.Empty || string.IsNullOrWhiteSpace(UserId))
                NavigationManager.NavigateTo("/");

            currentRestaurant = await RestaurantService.GetRestaurantByIdAsync(RestaurantId.ToString());

            if (currentRestaurant is null || currentRestaurant.Id == Guid.Empty)
                NavigationManager.NavigateTo("/");

            sessionStorageKey = $"{UserId}-{RestaurantId}";

            restaurantDishes = await DishService.GetCustomerDishesByRestaurantIdAsync(RestaurantId.ToString());

            foreach (var item in restaurantDishes.Where(x => x.ImageGuid is not null))
            {
                var pic = await PictureService.GetImage(item.ImageGuid);
                item.Image = pic.Item1;
                item.ImageType = pic.Item2;
            }

            currentOrder.RestaurantId = currentRestaurant.Id;
            currentOrder.UserId = UserId;

            bool hasSavedAddresses = customerAddresses != null && customerAddresses.Count > 0;
            addressPopupMode = hasSavedAddresses ? AddressPopupMode.Saved : AddressPopupMode.New;

            if (hasSavedAddresses)
            {
                customerAddresses.Insert(0, new AddressViewModel() { Address = "", Id = 0 });
            }

            //session storage            
            await InitSessionStorage();
        }

        private void AddDish(CustomerOrderDish dish)
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

        private void RemoveDish(CustomerOrderDish dish)
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
            hasSavedAddresses = customerAddresses != null && customerAddresses.Count > 0;

            if (hasSavedAddresses)
            {
                customerAddresses.Insert(0, new AddressViewModel() { Address = "- Choose an address -", Id = 0 });
            }

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

            if (string.IsNullOrWhiteSpace(currentOrder.Address) || currentOrder.Address == "- Choose an address -")
            {
                return;
            }

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

            currentOrder = new();
            await SessionStorage.DeleteAsync(sessionStorageKey);
            NavigationManager.NavigateTo($"/Home/MyRecentOrders", true);
        }

        private async Task InitSessionStorage()
        {
            var sessionOrderResult = await SessionStorage.GetAsync<Dictionary<int, int>>(sessionStorageKey);
            if (sessionOrderResult.Success)
            {
                sessionOrder = sessionOrderResult.Value;
            }

            if (!sessionOrder.Any() || sessionOrder.All(x => x.Value == 0))
            {
                sessionOrder.Clear();
                StartOrderWatchDog(token);
            }
            else
            {
                showResumeDialog = true;
            }
        }

        private async Task DeclineOldOrder()
        {
            showResumeDialog = false;
            sessionOrder.Clear();
            await SessionStorage.DeleteAsync(sessionStorageKey);

            StartOrderWatchDog(token);
        }

        private void AcceptOldOrder()
        {
            foreach (var orderItem in sessionOrder)
            {
                var dish = restaurantDishes.FirstOrDefault(x => x.Id == orderItem.Key);
                if (dish is not null)
                {
                    currentOrder.OrderItems.Add(dish, orderItem.Value);
                }
                else
                {
                    sessionOrder.Remove(orderItem.Key);
                }
            }

            showResumeDialog = false;

            StartOrderWatchDog(token);
        }

        private async Task StartOrderWatchDog(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000, cancellationToken);

                if (currentOrder.OrderItems.Count == 0 || currentOrder.OrderItems.All(x => x.Value == 0))
                {
                    await SessionStorage.DeleteAsync(sessionStorageKey);
                }
                else
                {
                    sessionOrder = currentOrder.OrderItems.Where(x => x.Value > 0).ToDictionary(x => x.Key.Id, x => x.Value);
                    await SessionStorage.SetAsync(sessionStorageKey, sessionOrder);
                }
            }
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }
    }

    public enum AddressPopupMode
    {
        New,
        Saved,
    }
}
