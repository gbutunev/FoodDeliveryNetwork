using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.SignalR.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace FoodDeliveryNetwork.SignalR
{
    public class OrderUpdateHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.GetId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User.GetId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                Context.Abort();
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateCustomerView(string userId, string orderId, OrderStatus newStatus)
        {
            //send data to group with key userId
            await Clients.Group(userId).SendAsync("UpdateCustomerView", orderId, newStatus);
        }        
    }
}
