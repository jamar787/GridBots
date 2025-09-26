using FastEndpoints;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Services;

namespace WarehouseDemoBackend.Endpoints
{
    public class UpdateWarehouseRequest
    {
        public float BorderWidth { get; set; }
        public float BorderHeight { get; set; }
        public int NumGridsX { get; set; }
        public int NumGridsY { get; set; }
    }

    public class UpdateWarehouseResponse
    {
        public string Message { get; set; }
    }

    public class UpdateWarehouseEndpoint : Endpoint<UpdateWarehouseRequest, UpdateWarehouseResponse>
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IHubContext<WarehouseHub> _hubContext;

        public UpdateWarehouseEndpoint(IWarehouseService warehouseService, IHubContext<WarehouseHub> hubContext)
        {
            _warehouseService = warehouseService;
            _hubContext = hubContext;
        }

        public override void Configure()
        {
            Post("/warehouse/update");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UpdateWarehouseRequest req, CancellationToken ct)
        {
            var warehouse = _warehouseService.Warehouse;

            warehouse.Resize(new Vector2(req.BorderWidth, req.BorderHeight));
            warehouse.UpdateNumGrids(new Vector2(req.NumGridsX, req.NumGridsY));
            warehouse.SetGridSizing();

            await _hubContext.Clients.All.SendAsync("WarehouseUpdated", warehouse, ct);
            await Send.OkAsync(new UpdateWarehouseResponse { Message = "Warehouse updated successfully" }, cancellation: ct);
        }
    }

}
