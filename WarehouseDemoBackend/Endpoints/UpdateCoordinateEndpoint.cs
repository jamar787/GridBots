using FastEndpoints;
using WarehouseDemoBackend.Services;

namespace WarehouseDemoBackend.Endpoints
{
    //public class UpdateCoordinateRequest
    //{
    //    public int Id { get; set; }
    //    public double Lat { get; set; }
    //    public double Lon { get; set; }
    //}

    //public class UpdateCoordinateResponse
    //{
    //    public bool Success { get; set; }
    //}

    //public class UpdateCoordinateEndpoint
    //    : Endpoint<UpdateCoordinateRequest, UpdateCoordinateResponse>
    //{
    //    private readonly ICoordinateService _coordService;

    //    public UpdateCoordinateEndpoint(ICoordinateService coordService)
    //    {
    //        _coordService = coordService;
    //    }

    //    public override void Configure()
    //    {
    //        Put("/api/coord/update");
    //        AllowAnonymous();  // Or add auth if needed
    //    }

    //    public override async Task HandleAsync(UpdateCoordinateRequest req, CancellationToken ct)
    //    {
    //        await _coordService.UpdateCoordinateAsync(req.Id, req.Lat, req.Lon, ct);
    //        //await SendAsync(new UpdateCoordinateResponse { Success = true }, cancellation: ct);
    //        await Send.ResponseAsync(new UpdateCoordinateResponse { Success = true }, cancellation: ct);
    //    }
    //}
}
