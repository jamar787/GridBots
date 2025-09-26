using FastEndpoints;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;
using WarehouseDemoBackend.Endpoints;
using WarehouseDemoBackend.Hubs;
using WarehouseDemoBackend.Models;
using WarehouseDemoBackend.Services;

public class AddBotRequest
{
    public float X { get; set; }
    public float Y { get; set; }
    public string DefaultColor { get; set; }
    public double StartingStepSpeed { get; set; }
    public BotEnums.OperationMode Mode { get; set; }
    public bool UseBrokenCycles { get; set; }
    public int BrokenCycleLimit { get; set; }
    public int BrokenCycleTarget { get; set; }
    public int BreakChance { get; set; }
    public int DirectionChangeTargetVal { get; set; }
    public int DirectionChangeChance { get; set; }
    public int IdleRollTargetVal { get; set; }
    public int IdleChangeLimit { get; set; }
}
public class AddBotResponse
{
    public string Message { get; set; }
}

public class AddBotEndpoint : Endpoint<AddBotRequest, AddBotResponse>
{
    private readonly IWarehouseService _warehouseService;
    private readonly IHubContext<WarehouseHub> _hubContext;

    public AddBotEndpoint(IWarehouseService warehouseService, IHubContext<WarehouseHub> hubContext)
    {
        _warehouseService = warehouseService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("/warehouse/bot/add");
        AllowAnonymous();
    }

    
    public override async Task HandleAsync(AddBotRequest req, CancellationToken ct)
    {
        var warehouse = _warehouseService.Warehouse;

        warehouse.AddNewBot(
            TopLeftStartingPos: new Vector2(req.X, req.Y),
            defaultColor: req.DefaultColor,
            startingStepSpeed: req.StartingStepSpeed,
            mode: req.Mode,
            useBrokenCycles: req.UseBrokenCycles,
            brokenCycleLimit: req.BrokenCycleLimit,
            brokenCycleTarget: req.BrokenCycleTarget,
            breakChance: req.BreakChance,
            directionChangeTargetVal: req.DirectionChangeTargetVal,
            directionChangeChance: req.DirectionChangeChance,
            idleRollTargetVal: req.IdleRollTargetVal,
            idleChangeLimit: req.IdleChangeLimit
        );

        // Broadcast update to all SignalR clients
        await _hubContext.Clients.All.SendAsync("WarehouseUpdated", warehouse, ct);
        await Send.OkAsync(new AddBotResponse { Message = "Bot added successfully" }, cancellation: ct);
        //await Send.ResponseAsync(new AddBotResponse { Message = "Bot added successfully" }, cancellation:ct);
        //await Send.ResponseAsync(new UpdateCoordinateResponse { Success = true }, cancellation: ct);
        //await SendOkAsync(ct);
    }
}

