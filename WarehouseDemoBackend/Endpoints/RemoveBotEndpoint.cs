using FastEndpoints;
using Microsoft.AspNetCore.SignalR;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Services;

namespace WarehouseDemoBackend.Endpoints
{
    public class RemoveBotRequest
    {
        public int BotId { get; set; }
    }
    public class RemoveBotResponse
    {
        public string Message { get; set; }
    }


    public class RemoveBotEndpoint : Endpoint<RemoveBotRequest, RemoveBotResponse>
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IHubContext<WarehouseHub> _hubContext;

        public RemoveBotEndpoint(IWarehouseService warehouseService, IHubContext<WarehouseHub> hubContext)
        {
            _warehouseService = warehouseService;
            _hubContext = hubContext;
        }

        public override void Configure()
        {
            Post("/warehouse/bot/remove");
            AllowAnonymous();
        }

        public override async Task HandleAsync(RemoveBotRequest req, CancellationToken ct)
        {
            var warehouse = _warehouseService.Warehouse;
            warehouse.RemoveBot(req.BotId);

            await _hubContext.Clients.All.SendAsync("WarehouseUpdated", warehouse, ct);
            await Send.OkAsync(new RemoveBotResponse { Message = "Bot removed successfully" }, cancellation: ct);
        }
    }

}
