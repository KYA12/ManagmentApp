using Microsoft.AspNetCore.SignalR;

namespace BakeryOrderManagmentSystem.API.Hubs
{
    public interface INotificationHub
    {
        Task NotifyProductDeleted(int productId);
    }
}
