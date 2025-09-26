using Microsoft.AspNetCore.SignalR;

namespace WarehouseDemoBackend.Hubs
{
    public class WarehouseHub : Hub
    {
        public async Task UpdateWarehouse(Models.Warehouse warehouse)
        {
            await Clients.All.SendAsync("WarehouseUpdated", warehouse);
        }
    }
}
