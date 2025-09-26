using Microsoft.AspNetCore.SignalR;
using System.Numerics;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Models;

namespace WarehouseDemoBackend.Services
{
        public interface IWarehouseService
        {
            Warehouse Warehouse { get; }
        void StartSimulation(IHubContext<WarehouseHub> hubContext, int intervalMs = 500);
        void StopSimulation();
        }
        public class WarehouseService : IWarehouseService
        {
            public Warehouse Warehouse { get; private set; }

        private CancellationTokenSource _cts;
            public WarehouseService()
            {

                Warehouse = new Warehouse(
                    border: new BoundingBox(new Vector2(0, 0), new Vector2(1200, 800)),
                    numGridsX: 12,
                    numGridsY: 8,
                    botSpeed: 0.25f,
                    botScalingFactor: .2);
            }
        public void StartSimulation(IHubContext<WarehouseHub> hubContext, int intervalMs = 500)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                return; // Already running

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    // Move all bots
                    Warehouse.StepForward();

                    // Broadcast updated warehouse state
                    await hubContext.Clients.All.SendAsync("WarehouseUpdated", Warehouse, token);

                    await Task.Delay(intervalMs, token);
                }
            }, token);
        }

        public void StopSimulation()
        {
            _cts?.Cancel();
        }
    }

    }

