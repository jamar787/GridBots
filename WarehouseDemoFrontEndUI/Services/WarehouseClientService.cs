using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using WarehouseDemoBackend.Models;

namespace WarehouseDemoFrontEndUI.Services
{
    public class WarehouseClientService
    {
        public Warehouse? Warehouse { get; private set;}
        private HubConnection _hubConnection;

        public event Action? OnWarehouseUpdated;

        public WarehouseClientService(NavigationManager nav)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(nav.ToAbsoluteUri("/warehouseHub"))
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<Warehouse>("WarehouseUpdated", (warehouse) =>
            {
                Warehouse = warehouse;
                OnWarehouseUpdated?.Invoke();
            });
        }

        public async Task StartAsync() => await _hubConnection.StartAsync();
    }
}
