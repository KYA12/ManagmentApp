using Microsoft.AspNetCore.SignalR;

namespace BakeryOrderManagmentSystem.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task NotifyProductDeleted(int productId)
        {
            // Notify all connected clients that a product has been deleted
            await Clients.All.SendAsync("ReceiveProductDeleted", productId);
        }
    }
}
